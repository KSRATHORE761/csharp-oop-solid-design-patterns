# Exception Handling: Answers

Try the questions in `02-Practice-Questions.md` first.

---

## Part A: Concept Questions

**1. `throw;` vs `throw ex;`**
`throw;` rethrows the current exception and **preserves the original stack trace**. `throw ex;` rethrows the same object but **resets the stack trace** to the rethrow line, so you lose where the error really started. Use `throw;` to rethrow, or `throw new X("msg", ex)` to wrap. `throw ex;` triggers analyzer warning CA2200.

**2. Does `finally` always run?**
Almost always, but not when: the process is killed or `Environment.FailFast` is called; there's a stack overflow; power failure or crash; an infinite loop/deadlock inside `try`; or an unhandled exception terminates the process (whether `finally` blocks run in that case is implementation-dependent).

**3. Catch order**
Most specific first, most general last. Placing a general `catch (Exception)` before a specific one is a **compile error** (CS0160) because the specific block would be unreachable.

**4. Exception filters**
`catch (X ex) when (condition)` catches only if the condition is true. Compared to `catch` + `if` + `throw;`: the filter runs **before the stack unwinds**, so the original stack and state are intact (better debugging and dumps), there is no rethrow, and it's cleaner. It also allows logging without catching (filter calls a method that logs and returns `false`).

**5. Custom exceptions**
Create one when callers need to handle a specific business failure differently (e.g. `InsufficientBalanceException`). Derive from `Exception` (or a suitable built-in). `ApplicationException` is legacy; Microsoft's guidance is not to derive from it because it adds no value and creates a pointless extra layer. Provide the standard constructors, add context properties, name it `...Exception`.

**6. Empty catch**
It swallows the error: the failure is hidden, state may be corrupted, and debugging becomes very hard. Instead: catch a specific exception you can handle, log it, rethrow (`throw;`) or wrap it, and let a global handler deal with what you can't recover from.

**7. Exceptions vs return values**
Exceptions are for **exceptional**, unexpected failures; they're costly (stack trace capture, unwinding). For **expected** failures (invalid input, not found), use return values: the **Try pattern** (`int.TryParse`, `dict.TryGetValue`) or a `Result<T>` type.

**8. `using`**
It compiles to `try { ... } finally { resource?.Dispose(); }`. If an exception occurs, `Dispose()` still runs, then the exception continues to propagate. `await using` does the same with `DisposeAsync()`.

**9. StackOverflow / unhandled exceptions**
`StackOverflowException` can't be caught; the process terminates. An unhandled exception on any thread terminates the process. `AppDomain.UnhandledException` allows last-chance logging but can't stop termination. In ASP.NET Core, exceptions in the request pipeline are caught by the server and turned into a 500 (or handled by your exception middleware).

**10. Exceptions in async methods**
The exception is captured and stored in the returned `Task` (status `Faulted`). It is **not** thrown at the call site; it's rethrown when you `await` the task (or access `.Result`/`.Wait()`). If the task is never awaited, the exception is "unobserved".

**11. `async void`**
The caller has no `Task` to await, so it can't catch the exception. The exception is raised on the `SynchronizationContext` or thread pool and can **crash the process**. Use `async void` only for event handlers; otherwise return `Task`.

**12. `Task.WhenAll` with multiple failures**
`await` throws only the **first** exception (in task order). To see all of them, keep the `WhenAll` task and read `task.Exception.InnerExceptions` (an `AggregateException`).

**13. `await` vs `.Wait()` / `.Result`**
`await` rethrows the **original** exception type. `.Wait()` / `.Result` wrap it in an `AggregateException` (use `.InnerException` or `GetAwaiter().GetResult()` to unwrap). Blocking with `.Result` / `.Wait()` can also cause deadlocks with a `SynchronizationContext`.

**14. InnerException / wrapping**
`InnerException` holds the exception that caused the current one. Wrap when you cross a layer boundary and want to present a meaningful higher-level error (e.g. `SqlException` → `OrderSaveFailedException`) **without losing the root cause**. Rethrow with `throw;` when you add nothing.

**15. Exception in constructors**
In a normal constructor, the object is never created; the caller gets the exception (a finalizer may still run on the partially constructed object). In a **static constructor**, the exception is wrapped in `TypeInitializationException` and the type is unusable for the rest of the app's lifetime.

**16. Exception in `finally` while another is in flight**
The new exception **replaces** the original; the original is lost (unless you captured it). So don't throw from `finally`; keep cleanup safe.

**17. Checked exceptions**
No. C# has no checked exceptions, so you're never forced to declare or catch them. The impact: you must document exceptions (XML docs `<exception>`) and rely on discipline and tests, so it's easy to forget to handle failure paths.

**18. Log and rethrow everywhere**
It produces duplicate log entries for one failure and clutters logs. Log **once**, at the boundary where the exception is finally handled (global handler, top of a background job), with full details. Lower layers should either handle it, or let it propagate / wrap it.

**19. Exceptions as control flow**
They're slow (stack capture + unwinding), make code harder to follow (hidden jumps), pollute logs and debugger "first chance" noise, and obscure the real failures. Use return values or the Try pattern for expected outcomes.

**20. Global handling in ASP.NET Core (.NET 8)**
Implement `IExceptionHandler`, register it with `AddExceptionHandler<T>()`, add `AddProblemDetails()`, and call `app.UseExceptionHandler()`. In the handler: log the exception, map exception types to status codes (e.g. not found → 404, validation → 400, otherwise 500), and return `ProblemDetails` without a stack trace. Use the developer exception page only in Development. (Older approach: custom middleware with try/catch around `next`.)

**21. Why both global and specific handling?**
They solve different problems. **Specific (local)** handling is for code that can *do something useful* with the failure: recover (retry, fallback), add context or translate (e.g. wrap `SqlException` into a domain exception), clean up (`using`/`finally`), or continue after a partial failure in a batch. Only code near the failure knows the context. **Global** handling is the safety net at the boundary: it catches everything unexpected, returns one consistent error format (`ProblemDetails`), logs **once** with full details and a trace id, maps exception types to status codes in one place, hides internals from clients, and avoids try/catch boilerplate in every method. The global handler can't recover or retry (it lacks context), and local handling can't cover every unforeseen failure. Also, background workers and queue consumers don't pass through the HTTP pipeline, so they need their own top-level try/catch. Rule of thumb: catch locally only if you can recover, add context, or clean up; otherwise let it bubble to the global handler.

**22. Ways to implement global handling in ASP.NET Core**
1. `app.UseExceptionHandler(...)`: the built-in middleware (with an error path or an inline lambda).
2. `IExceptionHandler` (.NET 8+) registered with `AddExceptionHandler<T>()` plus `AddProblemDetails()` and `app.UseExceptionHandler()`. **My preference**: it's testable, supports dependency injection, and you can register several handlers (they run in registration order; the first to return `true` wins).
3. Custom middleware that wraps `await next(context)` in try/catch (the older approach, still fine and common).
4. MVC exception filters (`IExceptionFilter`): work only for MVC controllers, **not** for minimal APIs or exceptions thrown in other middleware.
5. Related but different: `UseDeveloperExceptionPage` (Development only) and `UseStatusCodePages` (for status codes without exceptions).

**23. Why map status codes in the handler, not in the exception**
Domain and application layers shouldn't know about HTTP; an exception class with a status code couples your business code to the web layer. Mapping in one place makes it easy to change, and the same exceptions can be reused for other entry points (gRPC, background jobs, console tools). The downside is that the switch can grow; split it into several `IExceptionHandler`s or use a mapping table.

**24. Hierarchy and catch order**
Yes. `ArgumentNullException` and `ArgumentOutOfRangeException` derive from `ArgumentException`, and a catch for a base type catches all derived types. So you must order catch blocks from **most specific to most general**; a base-type catch placed first makes the later derived-type catch unreachable (compile error). `catch (Exception)` catches everything because all exceptions derive from `Exception`.

---

## Part B: Predict the Output

**B1:** `1`
The return value is captured before `finally` runs. Changing `x` in `finally` doesn't change it.

**B2:** `try finally return`
`try ` is printed, the return value is computed, `finally ` is printed, then the method returns and `WriteLine` prints `return`.

**B3:**
```
inner finally
outer catch: A
```
The inner `catch (ArgumentException)` doesn't match `InvalidOperationException`, so the inner `finally` runs and the exception propagates to the outer catch.

**B4:** **Compile error** (CS0160). A previous catch clause already catches all exceptions of this or a super type, so the `ArgumentException` block is unreachable. Put `ArgumentException` first.

**B5:** Prints `log`, then the exception is **not caught** (the filter returned `false`), so it propagates and, with no outer handler, the application terminates with an unhandled exception. `caught` is never printed.

**B6:** `1,2`
The reference to the list is returned; `finally` adds to the same object before the caller reads it.

**B7:**
```
after call
caught boom
```
The exception is stored in the Task and only thrown on `await`.

**B8:** `second`
The exception from `finally` replaces the original `first` exception.

**B9:** `AggregateException`. Its `InnerException` is the `InvalidOperationException`. (With `await t` instead of `t.Wait()`, you would catch `InvalidOperationException` directly.)

---

## Part C: Scenario and Design Questions

**C1. Stack traces leaking to clients**
- Add global exception handling (`IExceptionHandler` + `UseExceptionHandler`) and `AddProblemDetails()`.
- Log the full exception server-side (with a correlation/trace id).
- Return a generic `ProblemDetails` (status, title, trace id), no stack trace or internal messages.
- Developer exception page only in Development.
- Map known domain exceptions to proper 4xx codes.

**C2. Empty catch in code review**
It swallows all failures, including `OutOfMemoryException` and bugs. The caller thinks the order was saved. Fix: remove the try/catch and let the exception propagate to the global handler, or catch only the specific exception you can handle (e.g. `DbUpdateConcurrencyException`), log it, and rethrow or return a meaningful failure.

**C3. Intermittent payment timeouts**
- Catch specific transient exceptions (`TimeoutException`, `HttpRequestException`) using a filter.
- Use a resilience library (Polly / `Microsoft.Extensions.Resilience`): retry with exponential backoff + jitter, timeout, circuit breaker.
- Ensure **idempotency** (idempotency key) so retries don't charge twice.
- After retries are exhausted, wrap in a domain exception (`PaymentUnavailableException`) with the original as `InnerException`.
- Log once with context (order id, attempt count); show the user a friendly message and keep the order in a "payment pending" state.

**C4. Custom exception hierarchy**
- Base `DomainException : Exception` (optionally with an error code).
- Children: `OrderNotFoundException`, `InsufficientStockException`, `InvalidOrderStateException`, `ValidationException` (with a list of errors).
- The global handler maps: not found → 404, validation → 400, state/stock conflict → 409, unknown → 500 (logged, generic message).
- Keep hierarchy shallow; carry ids/values as properties, not just in strings.

**C5. Background worker with a bad message**
Wrap the **per-message** processing in try/catch so one failure doesn't end the loop. Log with message id; retry transient failures (limited), then send the message to a **dead-letter queue** / error store (poison message handling); only the exception boundary at the top of the loop should be broad. Don't swallow cancellation: let `OperationCanceledException` stop the worker on shutdown.

**C6. The `catch` snippet**
- `logger.LogError(ex.Message)` loses the stack trace and inner exceptions. Use `logger.LogError(ex, "message with context")`.
- `throw ex;` resets the stack trace. Use `throw;`.
- Catching `Exception` here may be too broad. Catch what you can handle.
- Log-and-rethrow causes duplicate logs when a higher layer logs too. Either log here and handle, or just rethrow and let the boundary log.
- No context (ids, operation) in the log message.
