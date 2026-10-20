# Exception Handling: Interview Questions and Answers

**How to use this:** Read each question and answer it **aloud, without looking**. Then read the answer below it.
Mark the ones you stumble on and revise those sections in `01-Exception-Handling-Notes.md`.

Target: 6+ years experience. Expect follow-ups like "why?" and "what happens if...?".

**Contents:** Part A: Concepts (24) · Part B: Predict the Output (9) · Part C: Scenarios (6)

---

## Part A: Concept Questions

### Q1. What is the difference between `throw;` and `throw ex;`? Which should you use and why?

**Answer:**

`throw;` rethrows the current exception and **preserves the original stack trace**. `throw ex;` rethrows the same object but **resets the stack trace** to the rethrow line, so you lose where the error really started. Use `throw;` to rethrow, or `throw new X("msg", ex)` to wrap. `throw ex;` triggers analyzer warning CA2200.

---

### Q2. Does `finally` always execute? Name at least three situations where it doesn't.

**Answer:**

Almost always, but not when: the process is killed or `Environment.FailFast` is called; there's a stack overflow; power failure or crash; an infinite loop/deadlock inside `try`; or an unhandled exception terminates the process (whether `finally` blocks run in that case is implementation-dependent).

---

### Q3. In what order should `catch` blocks be written? What happens if a general `catch (Exception)` comes before a specific one?

**Answer:**

Most specific first, most general last. Placing a general `catch (Exception)` before a specific one is a **compile error** (CS0160) because the specific block would be unreachable.

---

### Q4. What are exception filters (`catch ... when`)? Why use them instead of `catch` + `if` + `throw;`?

**Answer:**

`catch (X ex) when (condition)` catches only if the condition is true. Compared to `catch` + `if` + `throw;`: the filter runs **before the stack unwinds**, so the original stack and state are intact (better debugging and dumps), there is no rethrow, and it's cleaner. It also allows logging without catching (filter calls a method that logs and returns `false`).

---

### Q5. When should you create a custom exception? Which class should it derive from, and why not `ApplicationException`?

**Answer:**

Create one when callers need to handle a specific business failure differently (e.g. `InsufficientBalanceException`). Derive from `Exception` (or a suitable built-in). `ApplicationException` is legacy; Microsoft's guidance is not to derive from it because it adds no value and creates a pointless extra layer. Provide the standard constructors, add context properties, name it `...Exception`.

---

### Q6. What is wrong with an empty `catch` block? What should you do instead?

**Answer:**

It swallows the error: the failure is hidden, state may be corrupted, and debugging becomes very hard. Instead: catch a specific exception you can handle, log it, rethrow (`throw;`) or wrap it, and let a global handler deal with what you can't recover from.

---

### Q7. Exceptions vs return values: when do you use each? What is the Try pattern?

**Answer:**

Exceptions are for **exceptional**, unexpected failures; they're costly (stack trace capture, unwinding). For **expected** failures (invalid input, not found), use return values: the **Try pattern** (`int.TryParse`, `dict.TryGetValue`) or a `Result<T>` type.

---

### Q8. What does a `using` statement compile to? What happens if an exception occurs inside it?

**Answer:**

It compiles to `try { ... } finally { resource?.Dispose(); }`. If an exception occurs, `Dispose()` still runs, then the exception continues to propagate. `await using` does the same with `DisposeAsync()`.

---

### Q9. Can you catch a `StackOverflowException`? What happens to an unhandled exception in a .NET application?

**Answer:**

`StackOverflowException` can't be caught; the process terminates. An unhandled exception on any thread terminates the process. `AppDomain.UnhandledException` allows last-chance logging but can't stop termination. In ASP.NET Core, exceptions in the request pipeline are caught by the server and turned into a 500 (or handled by your exception middleware).

---

### Q10. How do exceptions behave in `async` methods? Where exactly is the exception thrown?

**Answer:**

The exception is captured and stored in the returned `Task` (status `Faulted`). It is **not** thrown at the call site; it's rethrown when you `await` the task (or access `.Result`/`.Wait()`). If the task is never awaited, the exception is "unobserved".

---

### Q11. Why is `async void` dangerous with respect to exceptions?

**Answer:**

The caller has no `Task` to await, so it can't catch the exception. The exception is raised on the `SynchronizationContext` or thread pool and can **crash the process**. Use `async void` only for event handlers; otherwise return `Task`.

---

### Q12. With `Task.WhenAll(t1, t2, t3)`, if two tasks fail, what does `await` throw? How do you get all the exceptions?

**Answer:**

`await` throws only the **first** exception (in task order). To see all of them, keep the `WhenAll` task and read `task.Exception.InnerExceptions` (an `AggregateException`).

---

### Q13. What is the difference in exception behavior between `await task` and `task.Wait()` / `task.Result`?

**Answer:**

`await` rethrows the **original** exception type. `.Wait()` / `.Result` wrap it in an `AggregateException` (use `.InnerException` or `GetAwaiter().GetResult()` to unwrap). Blocking with `.Result` / `.Wait()` can also cause deadlocks with a `SynchronizationContext`.

---

### Q14. What is `InnerException`? When should you wrap an exception instead of rethrowing it?

**Answer:**

`InnerException` holds the exception that caused the current one. Wrap when you cross a layer boundary and want to present a meaningful higher-level error (e.g. `SqlException` → `OrderSaveFailedException`) **without losing the root cause**. Rethrow with `throw;` when you add nothing.

---

### Q15. What happens when an exception is thrown inside a constructor? Inside a static constructor?

**Answer:**

In a normal constructor, the object is never created; the caller gets the exception (a finalizer may still run on the partially constructed object). In a **static constructor**, the exception is wrapped in `TypeInitializationException` and the type is unusable for the rest of the app's lifetime.

---

### Q16. What happens if an exception is thrown inside a `finally` block while another exception is already in flight?

**Answer:**

The new exception **replaces** the original; the original is lost (unless you captured it). So don't throw from `finally`; keep cleanup safe.

---

### Q17. Does C# have checked exceptions like Java? What is the practical impact?

**Answer:**

No. C# has no checked exceptions, so you're never forced to declare or catch them. The impact: you must document exceptions (XML docs `<exception>`) and rely on discipline and tests, so it's easy to forget to handle failure paths.

---

### Q18. Why is "log and rethrow at every layer" considered an anti-pattern? Where should you log?

**Answer:**

It produces duplicate log entries for one failure and clutters logs. Log **once**, at the boundary where the exception is finally handled (global handler, top of a background job), with full details. Lower layers should either handle it, or let it propagate / wrap it.

---

### Q19. Why shouldn't exceptions be used for normal control flow?

**Answer:**

They're slow (stack capture + unwinding), make code harder to follow (hidden jumps), pollute logs and debugger "first chance" noise, and obscure the real failures. Use return values or the Try pattern for expected outcomes.

---

### Q20. How would you implement global exception handling in an ASP.NET Core (.NET 8) Web API?

**Answer:**

Implement `IExceptionHandler`, register it with `AddExceptionHandler<T>()`, add `AddProblemDetails()`, and call `app.UseExceptionHandler()`. In the handler: log the exception, map exception types to status codes (e.g. not found → 404, validation → 400, otherwise 500), and return `ProblemDetails` without a stack trace. Use the developer exception page only in Development. (Older approach: custom middleware with try/catch around `next`.)

---

### Q21. Why do we need **both** global exception handling and handling at specific places? When do you use each?

**Answer:**

They solve different problems. **Specific (local)** handling is for code that can *do something useful* with the failure: recover (retry, fallback), add context or translate (e.g. wrap `SqlException` into a domain exception), clean up (`using`/`finally`), or continue after a partial failure in a batch. Only code near the failure knows the context. **Global** handling is the safety net at the boundary: it catches everything unexpected, returns one consistent error format (`ProblemDetails`), logs **once** with full details and a trace id, maps exception types to status codes in one place, hides internals from clients, and avoids try/catch boilerplate in every method. The global handler can't recover or retry (it lacks context), and local handling can't cover every unforeseen failure. Also, background workers and queue consumers don't pass through the HTTP pipeline, so they need their own top-level try/catch. Rule of thumb: catch locally only if you can recover, add context, or clean up; otherwise let it bubble to the global handler.

---

### Q22. What are the different ways to implement global exception handling in ASP.NET Core? Which do you prefer and why?

**Answer:**

1. `app.UseExceptionHandler(...)`: the built-in middleware (with an error path or an inline lambda).
2. `IExceptionHandler` (.NET 8+) registered with `AddExceptionHandler<T>()` plus `AddProblemDetails()` and `app.UseExceptionHandler()`. **My preference**: it's testable, supports dependency injection, and you can register several handlers (they run in registration order; the first to return `true` wins).
3. Custom middleware that wraps `await next(context)` in try/catch (the older approach, still fine and common).
4. MVC exception filters (`IExceptionFilter`): work only for MVC controllers, **not** for minimal APIs or exceptions thrown in other middleware.
5. Related but different: `UseDeveloperExceptionPage` (Development only) and `UseStatusCodePages` (for status codes without exceptions).

---

### Q23. Why map exceptions to HTTP status codes inside the global handler instead of putting the status code inside the exception class?

**Answer:**

Domain and application layers shouldn't know about HTTP; an exception class with a status code couples your business code to the web layer. Mapping in one place makes it easy to change, and the same exceptions can be reused for other entry points (gRPC, background jobs, console tools). The downside is that the switch can grow; split it into several `IExceptionHandler`s or use a mapping table.

---

### Q24. Does `catch (ArgumentException)` also catch `ArgumentNullException`? Why? What does the exception class hierarchy tell you about catch order?

**Answer:**

Yes. `ArgumentNullException` and `ArgumentOutOfRangeException` derive from `ArgumentException`, and a catch for a base type catches all derived types. So you must order catch blocks from **most specific to most general**; a base-type catch placed first makes the later derived-type catch unreachable (compile error). `catch (Exception)` catches everything because all exceptions derive from `Exception`.

---

## Part B: Predict the Output

### B1.

What does this print?

```csharp
static int Test()
{
    int x = 1;
    try { return x; }
    finally { x = 2; }
}
Console.WriteLine(Test());
```

**Answer:**

`1`
The return value is captured before `finally` runs. Changing `x` in `finally` doesn't change it.

---

### B2.

What is the output (in order)?

```csharp
static string M()
{
    try
    {
        Console.Write("try ");
        return "return";
    }
    finally
    {
        Console.Write("finally ");
    }
}
Console.WriteLine(M());
```

**Answer:**

`try finally return`
`try ` is printed, the return value is computed, `finally ` is printed, then the method returns and `WriteLine` prints `return`.

---

### B3.

What is the output?

```csharp
try
{
    try { throw new InvalidOperationException("A"); }
    catch (ArgumentException) { Console.WriteLine("inner catch"); }
    finally { Console.WriteLine("inner finally"); }
}
catch (Exception ex)
{
    Console.WriteLine("outer catch: " + ex.Message);
}
```

**Answer:**

```
inner finally
outer catch: A
```

The inner `catch (ArgumentException)` doesn't match `InvalidOperationException`, so the inner `finally` runs and the exception propagates to the outer catch.

---

### B4.

Will this compile? If not, why?

```csharp
try { DoWork(); }
catch (Exception ex) { Console.WriteLine("general"); }
catch (ArgumentException ex) { Console.WriteLine("argument"); }
```

**Answer:**

**Compile error** (CS0160). A previous catch clause already catches all exceptions of this or a super type, so the `ArgumentException` block is unreachable. Put `ArgumentException` first.

---

### B5.

What happens when this runs (no outer try/catch)?

```csharp
static bool Log(Exception e) { Console.WriteLine("log"); return false; }

try { throw new Exception("x"); }
catch (Exception ex) when (Log(ex)) { Console.WriteLine("caught"); }
```

**Answer:**

Prints `log`, then the exception is **not caught** (the filter returned `false`), so it propagates and, with no outer handler, the application terminates with an unhandled exception. `caught` is never printed.

---

### B6.

What does `Console.WriteLine(result)` print?

```csharp
static List<int> Get()
{
    var list = new List<int> { 1 };
    try { return list; }
    finally { list.Add(2); }
}
var result = string.Join(",", Get());
Console.WriteLine(result);
```

**Answer:**

`1,2`
The reference to the list is returned; `finally` adds to the same object before the caller reads it.

---

### B7.

What is the output?

```csharp
static async Task Main()
{
    Task t = ThrowAsync();
    Console.WriteLine("after call");
    try { await t; }
    catch (Exception ex) { Console.WriteLine("caught " + ex.Message); }
}
static async Task ThrowAsync() { throw new Exception("boom"); }
```

**Answer:**

```
after call
caught boom
```

The exception is stored in the Task and only thrown on `await`.

---

### B8.

What does the caller see?

```csharp
static void Run()
{
    try { throw new Exception("first"); }
    finally { throw new Exception("second"); }
}
try { Run(); }
catch (Exception ex) { Console.WriteLine(ex.Message); }
```

**Answer:**

`second`
The exception from `finally` replaces the original `first` exception.

---

### B9.

Which exception type is caught, and what is its inner exception?

```csharp
Task t = Task.Run(() => ThrowSync());   // ThrowSync throws InvalidOperationException
try { t.Wait(); }
catch (Exception ex) { Console.WriteLine(ex.GetType().Name); }
```

**Answer:**

`AggregateException`. Its `InnerException` is the `InvalidOperationException`. (With `await t` instead of `t.Wait()`, you would catch `InvalidOperationException` directly.)

---

## Part C: Scenario and Design Questions

### C1.

Your Web API returns full stack traces to clients on errors. How do you fix this properly?

**Answer: Stack traces leaking to clients**

- Add global exception handling (`IExceptionHandler` + `UseExceptionHandler`) and `AddProblemDetails()`.
- Log the full exception server-side (with a correlation/trace id).
- Return a generic `ProblemDetails` (status, title, trace id), no stack trace or internal messages.
- Developer exception page only in Development.
- Map known domain exceptions to proper 4xx codes.

---

### C2.

In a code review you see this. What's wrong, and how would you fix it?

```csharp
try { SaveOrder(order); }
catch (Exception) { }
```

**Answer: Empty catch in code review**

It swallows all failures, including `OutOfMemoryException` and bugs. The caller thinks the order was saved. Fix: remove the try/catch and let the exception propagate to the global handler, or catch only the specific exception you can handle (e.g. `DbUpdateConcurrencyException`), log it, and rethrow or return a meaningful failure.

---

### C3.

A call to a payment service fails intermittently with timeouts. How would you handle exceptions here (retry, wrapping, logging, user message)?

**Answer: Intermittent payment timeouts**

- Catch specific transient exceptions (`TimeoutException`, `HttpRequestException`) using a filter.
- Use a resilience library (Polly / `Microsoft.Extensions.Resilience`): retry with exponential backoff + jitter, timeout, circuit breaker.
- Ensure **idempotency** (idempotency key) so retries don't charge twice.
- After retries are exhausted, wrap in a domain exception (`PaymentUnavailableException`) with the original as `InnerException`.
- Log once with context (order id, attempt count); show the user a friendly message and keep the order in a "payment pending" state.

---

### C4.

Design a custom exception hierarchy for an order-management system. What goes in the base class, and how does the API layer map these to HTTP status codes?

**Answer: Custom exception hierarchy**

- Base `DomainException : Exception` (optionally with an error code).
- Children: `OrderNotFoundException`, `InsufficientStockException`, `InvalidOrderStateException`, `ValidationException` (with a list of errors).
- The global handler maps: not found → 404, validation → 400, state/stock conflict → 409, unknown → 500 (logged, generic message).
- Keep hierarchy shallow; carry ids/values as properties, not just in strings.

---

### C5.

A background worker processes messages from a queue. One message throws an exception. What should the worker do so that one bad message doesn't stop the others?

**Answer: Background worker with a bad message**

Wrap the **per-message** processing in try/catch so one failure doesn't end the loop. Log with message id; retry transient failures (limited), then send the message to a **dead-letter queue** / error store (poison message handling); only the exception boundary at the top of the loop should be broad. Don't swallow cancellation: let `OperationCanceledException` stop the worker on shutdown.

---

### C6.

A teammate wrote:

```csharp
catch (Exception ex)
{
    logger.LogError(ex.Message);
    throw ex;
}
```

List every issue you can find.

**Answer: The `catch` snippet**

- `logger.LogError(ex.Message)` loses the stack trace and inner exceptions. Use `logger.LogError(ex, "message with context")`.
- `throw ex;` resets the stack trace. Use `throw;`.
- Catching `Exception` here may be too broad. Catch what you can handle.
- Log-and-rethrow causes duplicate logs when a higher layer logs too. Either log here and handle, or just rethrow and let the boundary log.
- No context (ids, operation) in the log message.

---

## Self-Scoring

| Score | Meaning |
|-------|---------|
| 31 to 39 right | Interview-ready on this topic |
| 23 to 30 | Revise the weak sections and retry in 2 days |
| Below 23 | Re-read the notes, then retry |

(Part A: 24 questions, Part B: 9, Part C: 6. Count a question right if you could explain it and answer the likely follow-up.)
