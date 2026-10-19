# Exception Handling: Practice Questions

**How to use this:** Answer each question **aloud or in writing, without notes**. Then check `03-Answers.md`.
Mark the ones you stumble on and revise those sections.

Target: 6+ years experience. Expect follow-ups like "why?" and "what happens if...?".

---

## Part A: Concept Questions

1. What is the difference between `throw;` and `throw ex;`? Which should you use and why?
2. Does `finally` always execute? Name at least three situations where it doesn't.
3. In what order should `catch` blocks be written? What happens if a general `catch (Exception)` comes before a specific one?
4. What are exception filters (`catch ... when`)? Why use them instead of `catch` + `if` + `throw;`?
5. When should you create a custom exception? Which class should it derive from, and why not `ApplicationException`?
6. What is wrong with an empty `catch` block? What should you do instead?
7. Exceptions vs return values: when do you use each? What is the Try pattern?
8. What does a `using` statement compile to? What happens if an exception occurs inside it?
9. Can you catch a `StackOverflowException`? What happens to an unhandled exception in a .NET application?
10. How do exceptions behave in `async` methods? Where exactly is the exception thrown?
11. Why is `async void` dangerous with respect to exceptions?
12. With `Task.WhenAll(t1, t2, t3)`, if two tasks fail, what does `await` throw? How do you get all the exceptions?
13. What is the difference in exception behavior between `await task` and `task.Wait()` / `task.Result`?
14. What is `InnerException`? When should you wrap an exception instead of rethrowing it?
15. What happens when an exception is thrown inside a constructor? Inside a static constructor?
16. What happens if an exception is thrown inside a `finally` block while another exception is already in flight?
17. Does C# have checked exceptions like Java? What is the practical impact?
18. Why is "log and rethrow at every layer" considered an anti-pattern? Where should you log?
19. Why shouldn't exceptions be used for normal control flow?
20. How would you implement global exception handling in an ASP.NET Core (.NET 8) Web API?
21. Why do we need **both** global exception handling and handling at specific places? When do you use each?
22. What are the different ways to implement global exception handling in ASP.NET Core? Which do you prefer and why?
23. Why map exceptions to HTTP status codes inside the global handler instead of putting the status code inside the exception class?
24. Does `catch (ArgumentException)` also catch `ArgumentNullException`? Why? What does the exception class hierarchy tell you about catch order?

---

## Part B: Predict the Output

**B1.** What does this print?

```csharp
static int Test()
{
    int x = 1;
    try { return x; }
    finally { x = 2; }
}
Console.WriteLine(Test());
```

**B2.** What is the output (in order)?

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

**B3.** What is the output?

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

**B4.** Will this compile? If not, why?

```csharp
try { DoWork(); }
catch (Exception ex) { Console.WriteLine("general"); }
catch (ArgumentException ex) { Console.WriteLine("argument"); }
```

**B5.** What happens when this runs (no outer try/catch)?

```csharp
static bool Log(Exception e) { Console.WriteLine("log"); return false; }

try { throw new Exception("x"); }
catch (Exception ex) when (Log(ex)) { Console.WriteLine("caught"); }
```

**B6.** What does `Console.WriteLine(result)` print?

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

**B7.** What is the output?

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

**B8.** What does the caller see?

```csharp
static void Run()
{
    try { throw new Exception("first"); }
    finally { throw new Exception("second"); }
}
try { Run(); }
catch (Exception ex) { Console.WriteLine(ex.Message); }
```

**B9.** Which exception type is caught, and what is its inner exception?

```csharp
Task t = Task.Run(() => ThrowSync());   // ThrowSync throws InvalidOperationException
try { t.Wait(); }
catch (Exception ex) { Console.WriteLine(ex.GetType().Name); }
```

---

## Part C: Scenario and Design Questions

**C1.** Your Web API returns full stack traces to clients on errors. How do you fix this properly?

**C2.** In a code review you see this. What's wrong, and how would you fix it?

```csharp
try { SaveOrder(order); }
catch (Exception) { }
```

**C3.** A call to a payment service fails intermittently with timeouts. How would you handle exceptions here (retry, wrapping, logging, user message)?

**C4.** Design a custom exception hierarchy for an order-management system. What goes in the base class, and how does the API layer map these to HTTP status codes?

**C5.** A background worker processes messages from a queue. One message throws an exception. What should the worker do so that one bad message doesn't stop the others?

**C6.** A teammate wrote:

```csharp
catch (Exception ex)
{
    logger.LogError(ex.Message);
    throw ex;
}
```

List every issue you can find.

---

## Self-Scoring

| Score | Meaning |
|-------|---------|
| 31 to 39 right | Interview-ready on this topic |
| 23 to 30 | Revise the weak sections and retry in 2 days |
| Below 23 | Re-read the notes, run `Examples/Program.cs`, then retry |

(Part A: 24 questions, Part B: 9, Part C: 6. Count a question right if you could explain it and answer the likely follow-up.)
