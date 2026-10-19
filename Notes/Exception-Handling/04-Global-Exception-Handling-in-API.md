# Global Exception Handling in an ASP.NET Core API (.NET 8)

Goal: when something fails anywhere in the API, the client gets a **clean, consistent error response**, the server **logs it once**, and **no stack trace leaks**.

> "Class level / local handling" below means a `try/catch` inside the class or method where the failure happens.

---

## Part 1: Build it step by step

### Step 1: Create your exception classes

```csharp
// Base class for all business (expected) exceptions
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception inner) : base(message, inner) { }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, object key)
        : base($"{entity} with id '{key}' was not found.") { }
}

public class BadRequestException : DomainException
{
    public BadRequestException(string message) : base(message) { }
}

public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message) { }
}
```

These exceptions carry **no HTTP details**. Business code shouldn't know about status codes.

---

### Step 2: Throw them from your services

```csharp
public class OrderService(IOrderRepository repo)
{
    public async Task<Order> GetAsync(int id)
    {
        var order = await repo.FindAsync(id);
        if (order is null)
            throw new NotFoundException(nameof(Order), id);     // no try/catch needed here

        return order;
    }
}
```

Controllers stay clean:

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController(OrderService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> Get(int id)
        => Ok(await service.GetAsync(id));       // no try/catch
}
```

---

### Step 3: Write the global handler

```csharp
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // 1. Map exception type → HTTP status + title
        var (status, title) = exception switch
        {
            NotFoundException   => (StatusCodes.Status404NotFound,   "Resource not found"),
            BadRequestException => (StatusCodes.Status400BadRequest, "Bad request"),
            ConflictException   => (StatusCodes.Status409Conflict,   "Conflict"),
            _                   => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        // 2. Log once. Real failures as Error, expected business errors as Warning
        if (status >= 500)
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);
        else
            logger.LogWarning("Handled exception: {Message}", exception.Message);

        // 3. Build a safe response (no stack trace)
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            // Business errors: safe to show the message.
            // Unexpected errors: show details only in Development.
            Detail = status < 500 ? exception.Message
                   : env.IsDevelopment() ? exception.Message : null,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;      // true = "I handled it", stop looking for other handlers
    }
}
```

---

### Step 4: Register it in `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();   // register the handler
builder.Services.AddProblemDetails();                              // standard error format

var app = builder.Build();

app.UseExceptionHandler();      // ← add EARLY, before routing, auth, etc.

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

**Important:** middleware order matters. `UseExceptionHandler()` must come **early** so it wraps everything after it.

---

### Step 5: Test it

`GET /api/orders/42` (order doesn't exist) returns **404** with:

```json
{
  "title": "Resource not found",
  "status": 404,
  "detail": "Order with id '42' was not found.",
  "instance": "GET /api/orders/42",
  "traceId": "0HN4..."
}
```

An unexpected bug (e.g. `NullReferenceException`) returns **500** with a generic title, **no stack trace**. The full details are in the server log, matched by `traceId`.

---

## Part 2: Useful extras

### Several handlers (one per concern)

You can register more than one handler. They run **in the order registered**; the first one that returns `true` wins.

```csharp
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();   // handles only NotFoundException
builder.Services.AddExceptionHandler<ValidationExceptionHandler>(); // handles only validation errors
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();     // fallback for everything else
```

A specific handler returns `false` when the exception isn't its type, so the next one gets a chance:

```csharp
public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
{
    if (ex is not NotFoundException nf) return false;     // not mine, pass it on
    // ... write 404 response
    return true;
}
```

Use this when the single `switch` grows too large.

### Things the exception handler does NOT cover
- **Model validation with `[ApiController]`**: invalid request bodies return 400 automatically (via `ModelState`), no exception is involved.
- **Non-exception status codes** (e.g. a 404 for an unknown route): use `UseStatusCodePages`.
- **Background services and queue consumers**: they don't go through the HTTP pipeline. They need their own try/catch per unit of work.

### Client disconnected (cancellation)
If the client cancels the request you get an `OperationCanceledException`. This is normal, not a bug. Don't log it as an error (check `httpContext.RequestAborted.IsCancellationRequested`).

### Other ways to do global handling

| Approach | Notes |
|----------|-------|
| `IExceptionHandler` (.NET 8+) | **Recommended.** Clean, testable, supports dependency injection, chainable |
| `UseExceptionHandler(...)` with a lambda or path | Built-in, fine for simple cases |
| Custom middleware (try/catch around `next`) | Older approach, still common, works in all versions |
| MVC exception filter (`IExceptionFilter`) | Controllers **only**. Misses minimal APIs and other middleware |
| `UseDeveloperExceptionPage` | Development only, shows full details |

---

## Part 3: Global or local? How to choose

### Ask yourself these 4 questions, in order

```
1. Can I RECOVER here (retry, fallback, default value)?      → YES: handle LOCALLY
2. Can I ADD USEFUL CONTEXT or translate the exception?      → YES: catch, wrap with inner exception, rethrow
3. Do I need CLEANUP (connection, file, lock)?               → YES: using / finally
4. None of the above?                                        → DON'T catch. Let it bubble to the GLOBAL handler
```

### Common situations

| Situation | Where | What to do |
|-----------|-------|-----------|
| Order / user not found | **Global** | Service throws `NotFoundException`; the global handler returns 404 |
| Invalid business input | **Global** | Throw `BadRequestException` → 400 |
| Unexpected bug (`NullReferenceException`) | **Global** | Log it, return a generic 500 |
| Intermittent timeout calling a payment API | **Local** | Retry with backoff (Polly). If it still fails, wrap in `PaymentUnavailableException(ex)` and let the global handler return 503/500 |
| Third-party API down but you have a fallback | **Local** | Catch, use cached or default data |
| Closing a DB connection / file | **Local** | `using` or `finally` |
| Low-level DB error (`DbUpdateException`) | **Local** (repository) | Wrap into a domain exception with context (`OrderSaveFailedException`), then let it bubble up |
| Importing 1,000 rows, a few are bad | **Local** | try/catch **per row**, collect the failures, continue |
| Concurrency conflict (`DbUpdateConcurrencyException`) | **Local**, then **Global** | Retry or reload if possible. Otherwise throw `ConflictException` → 409 |
| Background worker processing messages | **Local** (top-level per message) | One bad message must not stop the loop. Log it and send it to a dead-letter queue |

### Side-by-side

| | Local (class / method level) | Global (middleware / handler) |
|---|---|---|
| **Purpose** | Recover, add context, clean up | Safety net for everything else |
| **Knows the context?** | Yes | No |
| **Can retry / continue?** | Yes | No, the request is already failing |
| **Where** | Close to the failure (repository, service, client wrapper) | Edge of the app (pipeline) |
| **Logging** | Usually none (or only if the exception is fully handled here) | **Log once**, with trace id |
| **Response to client** | Doesn't decide | Builds a consistent `ProblemDetails` |
| **Covers** | Only what you expect | Everything that bubbles up |

### Anti-patterns to avoid

```csharp
// ❌ try/catch in every controller action
[HttpGet("{id}")]
public async Task<IActionResult> Get(int id)
{
    try { return Ok(await service.GetAsync(id)); }
    catch (Exception ex) { return StatusCode(500, ex.Message); }   // duplicates the global handler and leaks messages
}

// ❌ swallowing
catch (Exception) { return null; }

// ❌ log + rethrow at every layer → the same error logged 4 times
catch (Exception ex) { logger.LogError(ex, "failed"); throw; }
```

### One-line answer for interviews

> "I handle exceptions locally only when I can recover, add context, or clean up. Everything else bubbles up to a global handler (`IExceptionHandler` with `ProblemDetails`) that logs once, maps the exception type to an HTTP status, and returns a safe response, so controllers stay clean and clients never see a stack trace."

---

## Quick checklist

- [ ] Custom exceptions derive from a `DomainException` base (no HTTP details inside)
- [ ] Services **throw**, they don't build HTTP responses
- [ ] `GlobalExceptionHandler : IExceptionHandler` maps type → status
- [ ] `AddExceptionHandler`, `AddProblemDetails`, `UseExceptionHandler()` registered, and **early**
- [ ] 5xx logged as Error with trace id; 4xx as Warning or Information
- [ ] No stack traces or internal messages in production responses
- [ ] Background workers have their own top-level try/catch
