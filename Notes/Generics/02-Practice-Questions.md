# Generics: Interview Questions and Answers

**How to use this:** Read each question and answer it **aloud, without looking**. Then read the answer below it.
Mark the ones you stumble on and revise those sections in `01-Generics-Notes.md`.

**Contents:** Part A: Concepts (30) · Part B: Predict the Output / Will It Compile? (13) · Part C: Coding and Scenarios (9)

---

## Part A: Concept Questions

### Q1. What are generics? What problems do they solve compared to using `object` or `ArrayList`?

**Answer:**

They let you write a class, method or interface with a placeholder type (`T`) and choose the actual type at the point of use. Compared to `object`/`ArrayList` they give **compile-time type safety**, **no casting**, **no boxing** for value types (better performance), and **code reuse** (no `IntList`, `StringList`...).

---

### Q2. What is boxing, and how do generics help avoid it?

**Answer:**

Boxing wraps a value type (e.g. `int`) in a heap-allocated `object`; unboxing extracts it. It costs allocation, GC pressure and casts. `List<int>` stores ints directly in its internal `int[]`, so there is no boxing. `ArrayList` boxes every int.

---

### Q3. What is the difference between an **open** generic type (`List<>`) and a **closed** (constructed) generic type (`List<int>`)?

**Answer:**

`List<>` (or `List<T>` as a definition) is an **open generic type**: the type argument isn't supplied. `List<int>` is a **closed constructed type**: all type arguments are supplied, and it's the only kind you can instantiate. Open types are used in reflection (`typeof(List<>)`) and DI registrations (`services.AddScoped(typeof(IRepository<>), typeof(Repository<>))`).

---

### Q4. What is a generic constraint? List the different kinds of constraints you can use.

**Answer:**

A constraint (`where T : ...`) tells the compiler what `T` supports, so you can use those members. Kinds: `struct`, `class`, `class?`, `notnull`, `unmanaged`, `new()`, a base class, an interface, another type parameter (`where T : U`), `Enum`, `Delegate`. Modern: `INumber<T>` etc. for generic math.

---

### Q5. What are the rules for combining constraints? Which one must come last?

**Answer:**

Combine with commas. `class`/`struct` must come **first**, then base class/interfaces, and `new()` must be **last**: `where T : class, IEntity, new()`. Each type parameter has its own `where` clause.

---

### Q6. Can you write `new T()` inside a generic class? What is needed? What about `new T(args)`?

**Answer:**

Only with the `new()` constraint (`where T : new()`), and only for a **parameterless** constructor. Without the constraint it's a compile error (CS0304). `new T(args)` is never allowed; use `Activator.CreateInstance(typeof(T), args)` or pass a factory delegate `Func<T>` / `Func<args, T>`.

---

### Q7. How do generics work at runtime in .NET? How is this different from Java's type erasure? What does the JIT do for value types vs reference types?

**Answer:**

.NET **reifies** generics: `List<int>` and `List<string>` are distinct runtime types, and the metadata is preserved. Java uses **type erasure**: generics exist only at compile time, and primitives need boxing. In .NET the JIT generates **separate specialized native code for each value-type instantiation** (fast, no boxing) and **one shared native code for all reference-type instantiations** (references are the same size).

---

### Q8. Does a static field in a generic class get shared between `Counter<int>` and `Counter<string>`?

**Answer:**

No. Each closed generic type has its **own** static fields. `Counter<int>.Count` and `Counter<string>.Count` are independent.

---

### Q9. Explain **covariance** and **contravariance** with an example each.

**Answer:**

- **Covariance** (`out`): a type that only *returns* `T` can be used where a more general `T` is expected. `IEnumerable<string>` → `IEnumerable<object>`.
- **Contravariance** (`in`): a type that only *accepts* `T` can be used where a more specific `T` is expected. `Action<object>` → `Action<string>`: a handler for any object can handle a string.

---

### Q10. Why can `IEnumerable<string>` be assigned to `IEnumerable<object>`, but `List<string>` cannot be assigned to `List<object>`?

**Answer:**

`IEnumerable<out T>` is declared covariant: you can only read from it, so it's always safe. `List<T>` is **invariant**: it both reads and writes. If `List<string>` could be treated as `List<object>`, you could add an `int` into a list of strings, breaking type safety.

---

### Q11. What do the `in` and `out` keywords mean on a type parameter? Where are they allowed?

**Answer:**

`out T`: `T` appears only in output positions (return values), making the interface or delegate covariant. `in T`: `T` appears only in input positions (parameters), making it contravariant. Allowed **only on interfaces and delegates**, not on classes or structs.

---

### Q12. Does variance work with value types, e.g. `IEnumerable<int>` → `IEnumerable<object>`? Why or why not?

**Answer:**

No. `IEnumerable<int>` can't become `IEnumerable<object>`, because variance requires an identity-preserving **reference conversion**, and `int` → `object` needs **boxing**, which changes representation.

---

### Q13. What is the problem with **array covariance**?

**Answer:**

`object[] arr = new string[2];` compiles, but `arr[0] = 123;` throws `ArrayTypeMismatchException` at **runtime**. The type check is deferred to runtime (and costs performance). It's a legacy design flaw; generic collections catch this at compile time.

---

### Q14. What does `default(T)` or `default` return?

**Answer:**

The default value of `T`: `0` for numeric types, `false` for `bool`, `null` for reference types and `Nullable<T>`, and a zero-initialized struct for structs (`DateTime.MinValue` for `DateTime`).

---

### Q15. Why can't you use `+` or `>` on an unconstrained `T`? How do you solve it?

**Answer:**

The compiler only knows `T` as `object` unless constrained, and `object` has no `+` or `>`. Solutions: constrain to `IComparable<T>` and use `CompareTo`; in .NET 7+/C# 11 use **generic math** (`where T : INumber<T>`); or pass a delegate such as `Func<T,T,T>`; `dynamic` is a slow last resort.

---

### Q16. When does generic method **type inference** work and when doesn't it?

**Answer:**

The compiler infers `T` from the **arguments** (`Swap(ref x, ref y)`). It **cannot infer from the return type alone**, so `Create<T>()` needs `Create<Order>()`. It also fails when arguments give conflicting or no information.

---

### Q17. What is the difference between `IComparable` and `IComparable<T>`? Why is the generic one preferred?

**Answer:**

`IComparable.CompareTo(object)` requires casting and **boxes** value types, with no compile-time type safety. `IComparable<T>.CompareTo(T)` is type-safe and avoids boxing. Same idea for `IEquatable<T>` vs `Equals(object)`.

---

### Q18. What are `Func`, `Action` and `Predicate`? How do they differ?

**Answer:**

Built-in generic delegates. `Action<T...>` returns `void`. `Func<T..., TResult>` returns `TResult` (the last type parameter). `Predicate<T>` takes `T` and returns `bool` (equivalent to `Func<T,bool>`, used in `List<T>.Find`, `RemoveAll`). LINQ uses `Func`.

---

### Q19. How does `int?` relate to generics?

**Answer:**

`int?` is syntax sugar for `Nullable<int>`, a **generic struct** declared as `struct Nullable<T> where T : struct`. That's why `string?`-style nullable value types only work for value types.

---

### Q20. What is the difference between `IEnumerable<T>`, `ICollection<T>`, `IList<T>` and `List<T>`? What should a method accept and what should it return?

**Answer:**

`IEnumerable<T>`: forward-only read. `ICollection<T>`: adds `Count`, `Add`, `Remove`. `IList<T>`: adds index access. `List<T>`: concrete implementation. `IReadOnlyList<T>` / `IReadOnlyCollection<T>`: read-only views. **Guideline:** accept the **most general** type needed as a parameter (`IEnumerable<T>`), and return the type that best describes what callers can do (`IReadOnlyList<T>`, or `List<T>` for internal code).

---

### Q21. What is a generic repository? What are its pros and cons?

**Answer:**

`IRepository<T>` with common CRUD methods. **Pros:** less duplication, consistent API, easy to mock. **Cons:** often a leaky or redundant abstraction (EF Core's `DbContext`/`DbSet` already is a repository + unit of work), can't express query-specific needs well, tempts you to expose `IQueryable`, and encourages "anemic" data access. A common compromise: use `DbContext` directly for simple apps, or specific repositories (`IOrderRepository`) for real query logic.

---

### Q22. What are the limitations of generics in C#?

**Answer:**

No `new T(args)`; no operators on unconstrained `T`; constructors/properties/indexers/events can't have their own type parameters; `class A<T> : T` not allowed; can't overload on constraints alone; statics are per closed type (which surprises people); variance only on interfaces and delegates with reference types.

---

### Q23. What does `where T : class` vs `where T : struct` vs `where T : notnull` mean?

**Answer:**

`class`: reference types only. `struct`: non-nullable value types only. `notnull`: any non-nullable type (value or reference); mostly about nullable reference type warnings.

---

### Q24. How do you register a generic repository `IRepository<T>` in ASP.NET Core DI? What is an open-generic registration?

**Answer:**

Register the **open** types once and the container creates closed versions on demand:

```csharp
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

Then injecting `IRepository<Order>` gives a `Repository<Order>`, and `IRepository<User>` gives a `Repository<User>`. Note the **empty angle brackets** in `typeof(IRepository<>)`. A specific closed registration (e.g. `IRepository<Order>` → `OrderRepository`) takes precedence over the open generic one. The same technique is used for MediatR behaviours: `services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))`.

---

### Q25. How does `List<T>` grow internally? What is the difference between `Count` and `Capacity`, and how do you avoid repeated resizing?

**Answer:**

`List<T>` wraps an array. When the array is full it allocates a new array **twice as big** (first allocation is 4, then 8, 16, ...) and copies the items, so `Add` is **amortized O(1)** even though one resize is O(n). `Count` is the number of items stored; `Capacity` is the size of the internal array. If you know the expected size, pass it to the constructor (`new List<int>(1000)`) to avoid resizing. Index access is O(1); `Insert`/`Remove` in the middle is O(n) because items shift.

---

### Q26. How does `Dictionary<TKey, TValue>` work internally? Why must `Equals` and `GetHashCode` be consistent?

**Answer:**

It's a **hash table**. On add or lookup, it calls `key.GetHashCode()`, maps the hash to a **bucket**, then compares keys inside that bucket using `Equals`. If two different keys land in the same bucket (a **collision**), the entries are chained and checked one by one. Average lookup/add/remove is **O(1)**; the worst case is O(n). It resizes when it fills up. Because the hash picks the bucket and `Equals` confirms the match, **equal objects must have the same hash code**. If they don't, a lookup can miss an existing key. Keys should also be **immutable**: if a key's hash code changes after insertion, the entry is lost in the wrong bucket.

---

### Q27. What is `IEquatable<T>` and why implement it?

**Answer:**

It adds a type-safe `bool Equals(T other)`. Compared to `Equals(object)`, it needs **no cast** and **no boxing** for structs, and it's faster. `Dictionary`, `HashSet` and `List.Contains` use it through `EqualityComparer<T>.Default`. When you implement it, also override `Equals(object)` and `GetHashCode()` consistently (`records` generate all of this automatically).

---

### Q28. What is the difference between `IEnumerable<T>` and `IQueryable<T>`? What is the `AsEnumerable()` / `ToList()` trap?

**Answer:**

`IEnumerable<T>` works **in memory** with delegates (`Func<T,bool>`). `IQueryable<T>` (which inherits from `IEnumerable<T>`) holds an **expression tree** (`Expression<Func<T,bool>>`) that a provider such as EF Core **translates to SQL**, so filtering happens in the database. The trap: calling `.ToList()` or `.AsEnumerable()` **before** `.Where(...)` loads the whole table and filters in memory. Both are **deferred**: the query runs when you enumerate it. Returning `IQueryable<T>` from a repository also leaks the data layer and may run after the `DbContext` is disposed.

---

### Q29. Give examples of generics in the frameworks you use day to day.

**Answer:**

`ILogger<T>` (category name = `T`), `IOptions<T>`, `DbSet<T>` and `Set<T>()`, `Task<T>` / `ValueTask<T>`, `ActionResult<T>`, `IEnumerable<T>` and LINQ, `Nullable<T>`, `Lazy<T>`, `Span<T>`, `HttpClient.GetFromJsonAsync<T>`, MediatR's `IRequestHandler<TRequest, TResponse>` and `IPipelineBehavior<,>`, and your own `IRepository<T>` / `Result<T>`. In an interview, add a sentence about a place **you** used one, e.g. "we had a generic `Result<T>` so services returned failures without exceptions".

---

### Q30. Explain generics in 30 seconds.

**Answer:**

"Generics let me write a class or method once with a type placeholder and pick the real type at use. I get compile-time type safety, no casting, no boxing for value types, and less duplicate code. `List<T>` is the classic example. Constraints (`where T : ...`) let me use members of `T`, and in .NET generics are kept at runtime, so `List<int>` and `List<string>` are separate types."

---

## Part B: Predict the Output / Will It Compile?

### B1.

What does this print?

```csharp
class Counter<T> { public static int Count; }

Counter<int>.Count++;
Counter<int>.Count++;
Counter<string>.Count++;
Console.WriteLine(Counter<int>.Count + " " + Counter<string>.Count);
```

**Answer:**

`2 1`
Each closed type has its own static field.

---

### B2.

Will this compile?

```csharp
List<object> objs = new List<string>();
```

**Answer:**

**No.** Compile error (CS0029). `List<T>` is invariant.

---

### B3.

Will this compile?

```csharp
IEnumerable<object> objs = new List<string> { "a", "b" };
```

**Answer:**

**Yes.** `IEnumerable<out T>` is covariant, and `string` is a reference type.

---

### B4.

What happens when this runs?

```csharp
object[] arr = new string[2];
arr[0] = 123;
```

**Answer:**

Compiles, but throws **`ArrayTypeMismatchException`** at runtime (array covariance).

---

### B5.

Will this compile? If not, how do you fix it?

```csharp
static T Max<T>(T a, T b)
{
    return a > b ? a : b;
}
```

**Answer:**

**No.** Operator `>` can't be applied to operands of type `T`. Fix:

```csharp
static T Max<T>(T a, T b) where T : IComparable<T>
    => a.CompareTo(b) > 0 ? a : b;
```

---

### B6.

Will this compile? If not, why?

```csharp
static T Create<T>()
{
    return new T();
}
```

**Answer:**

**No.** CS0304: cannot create an instance of `T` because it has no `new()` constraint. Fix: `static T Create<T>() where T : new()`.

---

### B7.

What does this print?

```csharp
Console.WriteLine(typeof(List<int>) == typeof(List<string>));
Console.WriteLine(typeof(List<int>).Name);
```

**Answer:**

```
False
List`1
```

Different closed types at runtime; `` `1 `` means one type parameter.

---

### B8.

Do both lines compile?

```csharp
Func<string> f = () => "hi";
Func<object> g = f;

Action<object> a = o => Console.WriteLine(o);
Action<string> b = a;
```

**Answer:**

**Yes, both compile.** `Func<out TResult>` is covariant (`Func<string>` → `Func<object>`), and `Action<in T>` is contravariant (`Action<object>` → `Action<string>`).

---

### B9.

What does each call print?

```csharp
static T GetDefault<T>() => default!;

Console.WriteLine(GetDefault<int>());
Console.WriteLine(GetDefault<string>() is null);
Console.WriteLine(GetDefault<bool>());
```

**Answer:**

```
0
True
False
```

---

### B10.

Will this compile?

```csharp
IEnumerable<object> objs = new List<int> { 1, 2, 3 };
```

**Answer:**

**No.** Variance doesn't work with value types (`int` → `object` needs boxing). Fix: use `IEnumerable<int>` or cast with `.Cast<object>()`.

---

### B11.

What is the difference between these two queries (EF Core)?

```csharp
var a = db.Orders.Where(o => o.Total > 100).ToList();
var b = db.Orders.AsEnumerable().Where(o => o.Total > 100).ToList();
```

**Answer:**

Query `a` is translated to SQL with a `WHERE Total > 100`, so only matching rows come from the database. In query `b`, `AsEnumerable()` switches to in-memory LINQ **before** the filter, so **all rows** are loaded (`SELECT ... FROM Orders` with no `WHERE`) and filtered in memory. Same result, much more work.

---

### B12.

What does this print?

```csharp
class Key
{
    public int Id;
    public override int GetHashCode() => Id;
    public override bool Equals(object? o) => o is Key k && k.Id == Id;
}

var k = new Key { Id = 1 };
var dict = new Dictionary<Key, string> { [k] = "a" };

k.Id = 2;
Console.WriteLine(dict.ContainsKey(k));
```

**Answer:**

`False`. The entry was stored in the bucket for hash `1`. After `k.Id = 2`, the lookup computes hash `2`, looks in a different bucket, and doesn't find it. The entry is effectively lost. Lesson: **don't mutate dictionary keys**.

---

### B13.

What does this print?

```csharp
var list = new List<int>();
Console.WriteLine(list.Capacity);

list.Add(1);
Console.WriteLine(list.Capacity);

for (int i = 0; i < 4; i++) list.Add(i);
Console.WriteLine(list.Capacity);
```

**Answer:**

```
0
4
8
```

A new list has no internal array (capacity 0). The first `Add` allocates capacity 4. After the loop the list holds 5 items, which exceeds 4, so the array doubles to 8.

---

## Part C: Coding and Scenario Questions

### C1.

Write a generic `Repository<T>` with `Add`, `GetById` and `GetAll`. What constraint do you need on `T` for `GetById`?

**Answer: Generic repository**

```csharp
public interface IEntity { int Id { get; } }

public class Repository<T> where T : class, IEntity
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);
    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);
    public IEnumerable<T> GetAll() => _items;
}
```

`GetById` needs the `IEntity` constraint (or a base class) so the compiler knows `T` has an `Id`.

---

### C2.

A teammate stores mixed data in `List<object>` and casts everywhere. How would you improve the design?

**Answer: `List<object>` with casts**

It has no type safety (runtime `InvalidCastException`), casts everywhere, and boxing for value types. Options: a **generic** type if all items share a type (`List<Order>`); a common **interface/base class** (`List<IShape>`) if items share behavior; a **discriminated/union-like** design or separate typed collections if they are truly different; and generic methods where the type varies by call.

---

### C3.

Design a `Result<T>` class (success, value, error). When would you use it instead of throwing exceptions?

**Answer: `Result<T>`**

A generic wrapper with `IsSuccess`, `Value`, `Error` (see the notes, section 14). Use it for **expected** failures (validation, not found, business rules), where exceptions would be slow and noisy. Keep exceptions for unexpected failures. The result is explicit, forces callers to handle failure, and is cheap.

---

### C4.

Is a generic repository over EF Core a good idea? What are the trade-offs?

**Answer: Generic repository over EF Core**

Trade-offs as in A21. EF Core's `DbContext` already implements unit of work and `DbSet<T>` behaves like a repository, so a generic wrapper often adds little. It helps for consistency, mocking, or hiding the ORM, but gets awkward for includes, projections and complex queries. Pragmatic answer: simple CRUD can use the `DbContext`; domain-specific queries go in specific repositories or query handlers (CQRS).

---

### C5.

Write a generic method `FindMax<T>(IEnumerable<T> items)`. What constraint is needed? How do you handle an empty collection?

**Answer: `FindMax<T>`**

```csharp
static T FindMax<T>(IEnumerable<T> items) where T : IComparable<T>
{
    using var e = items.GetEnumerator();
    if (!e.MoveNext())
        throw new InvalidOperationException("Sequence is empty");   // or return default / a Result

    T max = e.Current;
    while (e.MoveNext())
        if (e.Current.CompareTo(max) > 0) max = e.Current;

    return max;
}
```

Constraint: `IComparable<T>`. For an empty collection, decide: throw `InvalidOperationException` (like LINQ's `Max`), return `default`, or return a `Result<T>`/nullable.

---

### C6.

Why is `IEnumerable<out T>` covariant while `IList<T>` is invariant?

**Answer: `IEnumerable<out T>` vs `IList<T>`**

`IEnumerable<T>` only returns `T` (via `GetEnumerator`/`Current`), so `out` is safe. `IList<T>` both returns `T` (indexer get) and accepts `T` (`Add`, indexer set), so it can't be either `in` or `out` and must be invariant.

---

### C7.

You have `Process<T>(T item)` and you need to create a new `T` inside it. What are your options if `T` has no parameterless constructor?

**Answer: Creating `T` without a parameterless constructor**

Options: (1) pass a **factory delegate** (`Func<T>` or `Func<Args, T>`), the cleanest; (2) `Activator.CreateInstance(typeof(T), args)` (reflection, slower, no compile-time check); (3) restrict `T` to an interface with a static factory method (static abstract members, C# 11); (4) let the DI container create instances.

---

### C8.

Write a generic `MyStack<T>` with `Push`, `Pop`, `Peek` and a `TryPop`. How do you handle an empty stack and growth?

**Answer: Generic stack**

```csharp
public class MyStack<T>
{
    private T[] _items = new T[4];
    private int _count;

    public int Count => _count;

    public void Push(T item)
    {
        if (_count == _items.Length)
            Array.Resize(ref _items, _items.Length * 2);   // double when full

        _items[_count++] = item;
    }

    public T Pop()
    {
        if (_count == 0) throw new InvalidOperationException("Stack is empty");

        T item = _items[--_count];
        _items[_count] = default!;       // let the GC collect the object
        return item;
    }

    public T Peek()
    {
        if (_count == 0) throw new InvalidOperationException("Stack is empty");
        return _items[_count - 1];
    }

    public bool TryPop(out T item)
    {
        if (_count == 0) { item = default!; return false; }
        item = Pop();
        return true;
    }
}
```

Talking points: LIFO; array doubling gives amortized O(1) push; `InvalidOperationException` for misuse vs `TryPop` for the expected case (Try pattern); clearing the slot for the GC; not thread-safe (use `ConcurrentStack<T>` or a lock).

---

### C9.

Your app has 20 entities, each with a repository. How do you register them in DI without 20 lines, and how do you override one with a custom implementation?

**Answer: Open-generic registration**

```csharp
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));      // covers all entities
builder.Services.AddScoped<IRepository<Order>, OrderRepository>();            // specific override
```

The open-generic registration covers every `IRepository<T>`; the specific closed registration for `Order` takes precedence for that one type. Mention the debate from A21: for EF Core, a generic repository is often redundant, so use it only if it adds value (consistency, testing).

---

## Self-Scoring

| Score | Meaning |
|-------|---------|
| 42 to 52 right | Interview-ready on this topic |
| 31 to 41 | Revise the weak sections and retry in 2 days |
| Below 31 | Re-read the notes, write the code examples yourself, then retry |

(Part A: 30 questions, Part B: 13, Part C: 9. Count a question right if you could explain it and answer the likely follow-up.)
