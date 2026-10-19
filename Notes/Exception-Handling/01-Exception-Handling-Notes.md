# Exception Handling in C# (Simple Notes)

An **exception** is an object created when something goes wrong at runtime (divide by zero, file missing, null reference). When it is thrown, normal flow stops and .NET looks **up the call stack** for a `catch` that can handle it. If none is found, the app crashes.

```
Main() → ProcessOrder() → SaveToDb() → throws
                                  ↑
          .NET walks back up: SaveToDb → ProcessOrder → Main, looking for a catch
```

---

## 1. try / catch

```csharp
try
{
    var a = 5;
    var b = 0;
    Console.WriteLine(a / b);
}
catch (DivideByZeroException ex)
{
    Console.WriteLine("Error: " + ex.Message);   // Attempted to divide by zero.
}
```

- `try`: code that might fail.
- `catch`: runs only if a matching exception is thrown.
- Without the catch, the app would crash.

**Useful properties on the exception object**

| Property | Meaning |
|----------|---------|
| `Message` | What went wrong |
| `StackTrace` | Where it happened (the call chain) |
| `InnerException` | The original exception, if this one wraps another |
| `ex.ToString()` | Type + message + inner exceptions + stack trace. **Use this for logging** |

---

## 2. Multiple catch blocks and their order

```csharp
try
{
    var text = File.ReadAllText("data.txt");
    var data = ParseData(text);
}
catch (FileNotFoundException ex)        // most specific
{
    Console.WriteLine("File not found: " + ex.Message);
}
catch (FormatException ex)              // specific
{
    Console.WriteLine("Bad format: " + ex.Message);
}
catch (Exception ex)                    // most general, always LAST
{
    Console.WriteLine("Unexpected error: " + ex.Message);
}
```

- Catch blocks are checked **top to bottom**. The **first match wins**, the rest are skipped.
- Always go **specific → general**.

> **Interview trap:** If `catch (Exception)` comes first, the code **does not compile** (error CS0160), because the later blocks could never run.

**Common built-in exceptions**

| Exception | When it happens / when to throw it |
|-----------|-------------------------------------|
| `ArgumentNullException` | A required argument is `null` |
| `ArgumentOutOfRangeException` | Argument is outside the allowed range |
| `ArgumentException` | Argument is invalid in some other way |
| `InvalidOperationException` | Method called when the object is in the wrong **state** |
| `NotSupportedException` | Operation not supported at all |
| `NotImplementedException` | Code not written yet |
| `ObjectDisposedException` | Using an object after `Dispose()` |
| `KeyNotFoundException` | Dictionary key not found |
| `NullReferenceException`, `IndexOutOfRangeException`, `InvalidCastException`, `FormatException`, `DivideByZeroException` | Thrown by the runtime for common mistakes |

All of them derive from `System.Exception`. (`ArgumentNullException` and `ArgumentOutOfRangeException` derive from `ArgumentException`.)

### Exception class hierarchy

```
System.Object
 └── System.Exception
      ├── System.SystemException              (thrown by the runtime / framework)
      │    ├── ArgumentException
      │    │    ├── ArgumentNullException
      │    │    └── ArgumentOutOfRangeException
      │    ├── InvalidOperationException
      │    │    └── ObjectDisposedException
      │    ├── NullReferenceException
      │    ├── IndexOutOfRangeException
      │    ├── InvalidCastException
      │    ├── FormatException
      │    ├── ArithmeticException
      │    │    ├── DivideByZeroException
      │    │    └── OverflowException
      │    ├── IOException
      │    │    └── FileNotFoundException
      │    ├── NotSupportedException
      │    ├── NotImplementedException
      │    ├── KeyNotFoundException
      │    ├── TimeoutException
      │    └── OperationCanceledException
      │         └── TaskCanceledException
      ├── AggregateException                  (wraps one or more exceptions, e.g. from Task.Wait())
      ├── System.ApplicationException         (legacy, do NOT derive from it)
      └── YourCustomException                 (derive from Exception)
```

**Why the tree matters**
- Catching a **base** type also catches all its **children**. `catch (ArgumentException)` also catches `ArgumentNullException` and `ArgumentOutOfRangeException`.
- That is why catch order must be **specific → general**.
- `catch (Exception)` catches everything, because every exception derives from `Exception`.

---

## 3. finally

`finally` runs **whether or not** an exception happened. Use it for **cleanup** (close files, release connections).

```csharp
try
{
    var file = File.Open("file.txt", FileMode.Open);
    file.Close();
}
catch (FileNotFoundException ex)
{
    Console.WriteLine("File not found: " + ex.Message);
}
finally
{
    Console.WriteLine("Executing finally block");   // always runs
}
```

### try / finally (no catch)

Use this when you want to **clean up but let the exception continue up** to someone who can handle it.

```csharp
public void OpenFile()
{
    try
    {
        var file = File.Open("file.txt", FileMode.Open);
        file.Close();
    }
    finally
    {
        Console.WriteLine("Cleanup done");   // runs, then the exception keeps travelling up
    }
}

try { OpenFile(); }
catch (Exception ex) { Console.WriteLine("Caught from OpenFile: " + ex.Message); }
```

> `try` must have **at least one** `catch` or a `finally`.

### When does `finally` NOT run?
- Process is killed, power failure, or `Environment.FailFast()`
- Stack overflow (process dies)
- Infinite loop or deadlock inside `try`
- Unhandled exception that terminates the app (not guaranteed to run)

### Two rules for `finally`
- You **cannot** `return` / `break` / `continue` out of a `finally` block (compile error).
- **Don't throw inside `finally`.** The new exception **replaces** the original one, and the original is lost.

---

## 4. finally with return (favorite interview trap)

```csharp
static int Test()
{
    int x = 1;
    try
    {
        return x;      // value 1 is saved here
    }
    finally
    {
        x = 2;         // changes the variable, NOT the saved return value
    }
}
// Test() returns 1
```

**Order:** the return value is calculated → `finally` runs → the saved value is returned.

With a **reference type**, `finally` can change the *object* (the reference was saved, not a copy):

```csharp
static List<int> Test()
{
    var list = new List<int> { 1 };
    try { return list; }
    finally { list.Add(2); }      // caller receives [1, 2]
}
```

---

## 5. Rethrowing: `throw;` vs `throw ex;` vs `throw new`

After catching, you often want to pass the exception up the stack. There are **3 ways**:

```csharp
try
{
    var file = File.Open("file.txt", FileMode.Open);
}
catch (FileNotFoundException ex)
{
    Console.WriteLine("File not found: " + ex.Message);

    throw;                                        // 1. keep original stack trace   ✅
    // throw ex;                                  // 2. resets stack trace           ❌
    // throw new Exception("File not found", ex); // 3. wrap, keep original inside   ✅
}
```

| Form | What happens to the stack trace | When to use |
|------|----------------------------------|-------------|
| `throw;` | **Preserved** (points to the real origin) | Default choice when you just pass it up |
| `throw ex;` | **Reset** to this line, so the real origin is lost | Almost never |
| `throw new X("msg", ex)` | New exception, original saved in `InnerException` | Translating to a higher-level error with context |

> `throw;` only works **inside a catch block**.

### Adding info without changing the exception

```csharp
catch (FileNotFoundException ex)
{
    ex.Data.Add("OrderId", orderId);    // attach extra details
    throw;                              // original stack trace kept
}
```

### Wrapping with InnerException

```csharp
catch (SqlException ex)
{
    throw new OrderSaveFailedException($"Could not save order {orderId}", ex);
}
```

Inner exceptions let you trace back through the chain: "OrderSaveFailed, caused by SqlException".

> Don't use `ApplicationException` for this (or as a base class). It's legacy and adds nothing. Use a built-in type or your own custom exception.

---

## 6. Exception filters (`when`)

A filter adds a **condition** to a catch. The catch runs only if the condition is true.

```csharp
try
{
    await client.GetAsync(url);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    Console.WriteLine("Resource not found");
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
{
    Console.WriteLine("Slow down, retry later");
}
catch (Exception ex)
{
    Console.WriteLine("Something else: " + ex.Message);
}
```

**Why use a filter instead of `catch` + `if` + `throw;`?**
- The filter is checked **before the stack unwinds**, so the original stack and state stay intact (better debugging and crash dumps).
- No rethrow needed. The code is cleaner.

**Tip:** filter on exception **type or properties** (like `StatusCode`). Avoid `ex.Message.Contains("...")` in real code, since messages can change.

**Popular pattern:**

```csharp
catch (Exception ex) when (ex is not OperationCanceledException)
```

**Trick: log without catching**

```csharp
catch (Exception ex) when (Log(ex))     // Log() returns false → NOT caught here
{
}
static bool Log(Exception ex) { logger.LogError(ex, "error"); return false; }
```

> **Interview trap:** if the filter returns `false`, the exception is **not caught** and keeps going up.

---

## 7. Custom exceptions

Create your own when callers need to handle a **specific business failure** differently (e.g. user not found, insufficient balance).

```csharp
public class UserNotFoundException : Exception
{
    public UserNotFoundException() { }

    public UserNotFoundException(string message)
        : base(message) { }

    public UserNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}
```

Using it:

```csharp
public async Task<User> GetUserByEmailAsync(string email)
{
    var user = await FindUserByEmailAsync(email);
    if (user is null)
        throw new UserNotFoundException($"User with email {email} was not found.");

    return user;
}
```

**Rules**
- Name ends with `Exception`.
- Derive from `Exception` (**not** `ApplicationException`).
- Provide the **3 standard constructors**: empty, message, message + inner exception.
- Add properties for useful context if needed (e.g. `OrderId`, `Balance`).
- Don't create one if a built-in exception already fits.
- Keep any hierarchy small, e.g. one `DomainException` base with a few children.

**Modern guard-clause helpers**

```csharp
ArgumentNullException.ThrowIfNull(order);                  // .NET 6+
ArgumentException.ThrowIfNullOrEmpty(name);                // .NET 7+
ArgumentOutOfRangeException.ThrowIfNegative(quantity);     // .NET 8+
```

---

## 8. using and exceptions

`using` guarantees cleanup even when an exception occurs. It is shorthand for `try/finally`.

```csharp
using (var conn = new SqlConnection(cs))
{
    conn.Open();
    // even if an exception happens here, Dispose() still runs
}

// same thing, written by the compiler:
var conn = new SqlConnection(cs);
try { /* ... */ }
finally { conn?.Dispose(); }
```

```csharp
using var file = File.OpenRead(path);        // disposed at the end of the scope
await using var res = new AsyncResource();   // for IAsyncDisposable
```

---

## 9. Exceptions vs return values

Throwing an exception is **slow** (captures the stack trace, unwinds the stack). Don't use it for normal flow.

```csharp
// ❌ exceptions as control flow
try { number = int.Parse(input); }
catch (FormatException) { number = 0; }

// ✅ Try pattern
if (!int.TryParse(input, out var number))
    number = 0;
```

- **Exceptions:** unexpected failures and broken contracts.
- **Return values (Try pattern, `Result<T>`):** things that are expected to fail sometimes (bad user input, "not found").

---

## 10. Exceptions in async code

```csharp
public async Task LoadAsync()
{
    throw new InvalidOperationException("boom");
}

Task t = LoadAsync();         // NO exception here. It is stored inside the Task
await t;                      // exception is thrown HERE
```

| Situation | What happens |
|-----------|--------------|
| `async Task` method throws | Exception is stored in the Task (`Faulted`) and **rethrown when you `await`** |
| `await task` | Throws the **original** exception |
| `task.Wait()` / `task.Result` | Throws `AggregateException` **wrapping** the original (and can deadlock) |
| `Task.WhenAll(t1, t2)` with several failures | `await` throws only the **first**; all are in `whenAllTask.Exception.InnerExceptions` |
| Task never awaited and it failed | "Unobserved exception" (does not crash the process by default since .NET 4.5) |
| Cancellation | `OperationCanceledException` is normal, not an error. Often filtered out |

```csharp
var all = Task.WhenAll(t1, t2);
try { await all; }
catch (Exception)
{
    foreach (var e in all.Exception!.InnerExceptions)   // see every failure
        Log(e);
}
```

### async void: the known evil

```csharp
public async void OpenFileAsync()      // ❌ avoid
{
    throw new Exception("boom");
}
```

- It returns no `Task`, so the caller **cannot catch** its exception.
- The exception is raised on the `SynchronizationContext` / thread pool and can **crash the whole process**.
- Use `async void` **only for event handlers**. Everywhere else return `Task`.

---

## 11. Unhandled exceptions and special cases

- An unhandled exception on any thread **terminates the app**.
- Last-chance logging only (can't stop the crash): `AppDomain.CurrentDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException`.
- `StackOverflowException` **cannot be caught**. The process dies.
- Exception in a **constructor**: the object isn't created; the caller gets the exception.
- Exception in a **static constructor**: wrapped in `TypeInitializationException`; the type is unusable for the app's lifetime.
- Never throw from a **finalizer** (crashes the process) or, ideally, from `Dispose`.
- C# has **no checked exceptions** (unlike Java). You're never forced to catch or declare them.

---

## 12. Global exception handling in ASP.NET Core (.NET 8)

Instead of try/catch in every controller, handle exceptions **once**, at the edge of the app.

**Step 1: write the handler**

```csharp
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Unhandled exception");          // log once, with details

        var (status, title) = exception switch
        {
            UserNotFoundException => (StatusCodes.Status404NotFound, "User not found"),
            ValidationException   => (StatusCodes.Status400BadRequest, "Validation failed"),
            _                     => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(
            new ProblemDetails { Status = status, Title = title }, ct);   // no stack trace for the client

        return true;     // true = handled
    }
}
```

**Step 2: register it in `Program.cs`**

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();      // add early in the pipeline
```

**What you get**
- Every unhandled exception ends up in one place.
- Same JSON error format for every endpoint (`ProblemDetails`).
- Exception type → HTTP status mapping in one spot.
- Clients never see stack traces or internal details.
- The developer exception page is for **Development only**.

*(Older way: a custom middleware that wraps `await next(context)` in try/catch.)*

---

## 13. Why do we need both global and specific handling?

They solve **different problems**.

### Specific (local) handling: at the place of failure
Catch locally **only if you can do something useful**:

| Reason | Example |
|--------|---------|
| **Recover** | Retry a timeout, fall back to cached data |
| **Add context / translate** | Catch `SqlException` in the repository, throw `OrderSaveFailedException(orderId, ex)` |
| **Clean up** | `using` / `finally` |
| **Partial failure** | One item in a batch fails: log it, continue with the rest |
| **Specific business reaction** | Catch `InsufficientBalanceException` and show "add funds" |

Only the code near the failure knows the context, so only it can decide **how to recover**.

### Global handling: the safety net
Handles **everything else** in one place:

- Catches the **unexpected** (bugs, unforeseen failures).
- **Consistent** error response for all endpoints.
- **Logs once**, with a trace id.
- One **mapping** of exception → status code.
- **No try/catch boilerplate** in every method.
- **Security:** hides stack traces and internals from clients.

### What each one can't do
- The global handler **can't recover or retry**. It has no context, and the request is already failing.
- Local handling **can't cover everything**, and try/catch everywhere makes code messy.
- The global handler only sees **HTTP requests**. Background workers and queue consumers need **their own top-level try/catch** per unit of work.

> **Rule of thumb:** Catch locally if you can **recover, add context, or clean up**. Otherwise let it bubble up to the global handler.

### Example flow

```
Controller → OrderService → OrderRepository → Database
                                   │
              DbUpdateException    │  caught here, wrapped with context:
                                   ▼  throw new OrderSaveFailedException(orderId, ex)
                    (bubbles up, no catch in service or controller)
                                   ▼
                          Global handler:
                           • logs once (with inner exception + trace id)
                           • maps to 500 / 409
                           • returns ProblemDetails (no stack trace)
```

---

## 14. Best practices

**Do**
- Catch **specific** exceptions before general ones.
- Validate arguments early (guard clauses).
- Use `throw;` to rethrow, or wrap with an inner exception.
- Use `using` / `finally` for cleanup.
- Log detailed information (`ex.ToString()`), **once**, at the boundary.
- Use the Try pattern for expected failures.
- Use exceptions only for things that are genuinely unexpected or beyond your control (file I/O, network, database).

**Don't**
- Leave an empty `catch { }` (it hides bugs).
- Catch `Exception` everywhere. Do it only at boundaries (global handler, top of a background job).
- Use exceptions for normal control flow.
- Use `throw ex;`.
- Log **and** rethrow at every layer (duplicate logs).
- Throw from `finally`, `Dispose`, or finalizers.
- Send stack traces or sensitive data to clients.
- Throw or derive from `ApplicationException`.

---

## 15. Interview traps: quick list

1. `finally` + `return`: the return value is already saved. (Reference types can still be modified.)
2. `throw;` keeps the stack trace. `throw ex;` resets it.
3. Catch order: specific first, otherwise a compile error.
4. A filter (`when`) that returns `false` means the exception is **not caught**, and the stack is intact.
5. `await` throws the original exception. `.Wait()` / `.Result` wrap it in `AggregateException`.
6. `Task.WhenAll`: `await` throws only the **first** exception.
7. `async void` exceptions can crash the process.
8. Throwing inside `finally` **replaces** the original exception.
9. Static constructor failure → `TypeInitializationException`.
10. `StackOverflowException` can't be caught.
11. No checked exceptions in C#.
12. Exceptions are slow, so don't use them for control flow.
13. Global handler = safety net (can't recover). Local handler = recover, add context, clean up.

---

## Quick cheat sheet

| Concept | One-liner |
|---------|-----------|
| try / catch | Run risky code, handle failure |
| Catch order | Specific → general |
| finally | Always cleanup (except crash / kill) |
| try / finally | Cleanup but let the exception continue up |
| `throw;` | Rethrow, keep stack trace ✅ |
| `throw ex;` | Rethrow, reset stack trace ❌ |
| `throw new X(msg, ex)` | Wrap, keep the cause in `InnerException` |
| Filter `when` | Catch only if the condition is true |
| Custom exception | Derive from `Exception`, 3 constructors |
| `using` | `try/finally` + `Dispose()` |
| Async | Stored in the Task, thrown on `await` |
| `async void` | Avoid, can crash the app |
| Global handler | `IExceptionHandler` + `ProblemDetails`, log once |
| Local handler | Only to recover, add context, or clean up |
| Try pattern | `TryParse` instead of try/catch for expected failures |
