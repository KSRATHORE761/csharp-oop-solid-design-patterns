# Generics in C# (Simple Notes)

**Big idea:** Write a class or method **once** with a placeholder type (`T`). Choose the real type **when you use it**.

Think of `T` like a **cookie cutter template**: the template is written once, and you get a real "int version" or "string version" when you use it.

**Contents:** 1 Why · 2 Class · 3 Method · 4 Interface · 5 Constraints · 6 default · 7 Under the hood · 8 Boxing · 9 Collections (List, Dictionary) · 10 Equality · 11 Delegates · 12 Variance · 13 IEnumerable vs IQueryable · 14 Frameworks and DI · 15 Practical examples · 16 Exercise · 17 Limitations · 18 Traps · 19 30-second answers · 20 Cheat sheet

> Each section ends with a **✅ Revise** box, so you can revise quickly by reading only those.

---

## 1. Why generics? (the problem)

**Without generics (`ArrayList`):**

```csharp
ArrayList list = new ArrayList();
list.Add(10);
list.Add("hello");              // compiles: any type allowed
int x = (int)list[1];           // 💥 InvalidCastException at RUNTIME
```

**With generics (`List<int>`):**

```csharp
List<int> list = new List<int>();
list.Add(10);
list.Add("hello");              // ❌ compile error
int x = list[0];                // no cast, no boxing
```

| Benefit | Meaning |
|---------|---------|
| **Type safety** | Mistakes caught at compile time |
| **No casting** | No `(int)list[0]` |
| **No boxing** | `int` stored directly, so it's faster |
| **Code reuse** | One `List<T>` instead of `IntList`, `StringList`, `OrderList`... |

> ✅ **Revise:** Generics = type safety + no casting + no boxing + reuse.

---

## 2. Generic class

```csharp
public class Box<T>
{
    public T Value { get; set; }

    public Box(T value) => Value = value;

    public void Show() => Console.WriteLine($"Box has {Value}");
}

var intBox = new Box<int>(10);
var strBox = new Box<string>("hello");
intBox.Show();     // Box has 10
strBox.Show();     // Box has hello
```

- `T` = **type parameter** (placeholder). Names: `T`, or `TKey`, `TValue`, `TEntity`, `TResult`.
- `Box<>` = **open** generic type. `Box<int>` = **closed** (constructed) type. You can only create objects of closed types.
- More than one type parameter is fine:

```csharp
public class Pair<TKey, TValue>
{
    public TKey Key { get; }
    public TValue Value { get; }
    public Pair(TKey key, TValue value) { Key = key; Value = value; }
}

var p = new Pair<string, int>("age", 30);
```

> ✅ **Revise:** `T` is a placeholder. Open = `List<>`, closed = `List<int>`.

---

## 3. Generic method

```csharp
static void Swap<T>(ref T a, ref T b)
{
    T temp = a;
    a = b;
    b = temp;
}

int x = 1, y = 2;
Swap(ref x, ref y);          // T inferred as int
Swap<int>(ref x, ref y);     // or write it explicitly

string s1 = "a", s2 = "b";
Swap(ref s1, ref s2);        // same method, now T = string
```

- The compiler **infers** `T` from the **arguments**.
- It **cannot infer `T` from the return type**: `Create<T>()` must be called as `Create<Order>()`.
- A generic method can live in a normal (non-generic) class.

> ✅ **Revise:** Inference comes from arguments, never from the return type.

---

## 4. Generic interface and inheritance

```csharp
public interface IRepository<T>
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T item);
}

public class OrderRepository : IRepository<Order> { /* ... */ }          // closed: T fixed to Order
public class InMemoryRepository<T> : IRepository<T> { /* ... */ }        // still generic
```

Inheriting from a generic class works the same way:

```csharp
public class Repository<T> { }

public class OrderRepository : Repository<Order> { }       // closed
public class CachedRepository<T> : Repository<T> { }       // open: passes T along
```

Problem: inside `IRepository<T>`, the compiler doesn't know `T` has an `Id`. That's what **constraints** solve.

> ✅ **Revise:** A generic type can implement/inherit generics, either fixing `T` or passing it on.

---

## 5. Constraints (`where`)

Inside generic code the compiler only knows `T` is an `object`. A **constraint** tells it more.

```csharp
static T Max<T>(T a, T b)
{
    return a > b ? a : b;                  // ❌ operator > can't be used on T
}

static T Max<T>(T a, T b) where T : IComparable<T>
{
    return a.CompareTo(b) > 0 ? a : b;     // ✅
}

Max(3, 7);                // 7
Max("apple", "pear");     // pear
```

### Types of constraints

| Constraint | `T` must be... |
|------------|----------------|
| `where T : struct` | A non-nullable **value type** |
| `where T : class` | A **reference type** |
| `where T : class?` | A reference type (nullable or not) |
| `where T : notnull` | A non-nullable type (value or reference) |
| `where T : unmanaged` | A value type with no references inside |
| `where T : new()` | Has a public **parameterless constructor** |
| `where T : BaseClass` | `BaseClass` or derived from it |
| `where T : IInterface` | Implements that interface |
| `where T : U` | Derived from another type parameter `U` |
| `where T : Enum` / `Delegate` | An enum / delegate type |

### Rules

- Combine with commas: `where T : class, IEntity, new()`
- **Order:** `class` / `struct` first, then base class and interfaces, and `new()` **last**.
- One `where` per type parameter: `where TKey : notnull where TValue : class`
- Two overloads that differ **only by constraints** are not allowed.

### The `new()` constraint

```csharp
static T Create<T>() where T : new()
{
    return new T();       // ✅ allowed only because of new()
}
```

- Without `new()` → **compile error** (CS0304).
- Only a **parameterless** constructor. `new T(args)` is **not allowed**. Use a factory `Func<T>` or `Activator.CreateInstance(typeof(T), args)`.

### Example: generic repository with a constraint

```csharp
public interface IEntity
{
    int Id { get; }
}

public class Repository<T> where T : class, IEntity
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);

    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);   // Id is known thanks to IEntity

    public IEnumerable<T> GetAll() => _items;
}

public class Product : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

var repo = new Repository<Product>();
repo.Add(new Product { Id = 1, Name = "Pen" });
```

### Operators on `T` (generic math, .NET 7+)

```csharp
static T Add<T>(T a, T b) where T : INumber<T> => a + b;

Add(2, 3);        // 5
Add(1.5, 2.5);    // 4.0
```

Older workarounds: pass a `Func<T,T,T>`, or use `dynamic` (slow).

> ✅ **Revise:** Constraint = tells the compiler what `T` can do. `new()` last, parameterless only. Use `IComparable<T>` for comparing, `INumber<T>` for math.

---

## 6. `default` and comparing `T`

```csharp
T value = default;       // 0 for numbers, false for bool, null for reference types
```

| `T` is... | `default` gives |
|-----------|-----------------|
| `int` | `0` |
| `bool` | `false` |
| `string`, any class | `null` |
| `DateTime` | `DateTime.MinValue` |
| a struct | all fields set to their defaults |

Comparing two `T` values:

```csharp
if (a == b) { }                                  // ❌ not allowed for unconstrained T
EqualityComparer<T>.Default.Equals(a, b);        // ✅ correct way
if (value == null) { }                           // allowed (always false for non-nullable value types)
```

> ✅ **Revise:** `default` = 0 / false / null. Compare `T` with `EqualityComparer<T>.Default`, not `==`.

---

## 7. How generics work under the hood (simple version)

**Picture it:** `List<T>` is a **template**. When you write `List<int>` and `List<string>`, .NET creates **two separate real types** from that template.

### Three facts to remember

**1. Generics are kept at runtime ("reified").**
`List<int>` and `List<string>` are different types while the program runs. Java erases the type at compile time, and .NET does not.

```csharp
typeof(List<int>) == typeof(List<string>);   // false
typeof(List<>);                              // open generic type
typeof(List<int>).Name;                      // "List`1"  (1 = number of type parameters)
```

**2. The JIT compiles differently for value types and reference types.**

| Type argument | What the JIT does |
|---------------|-------------------|
| **Value type** (`int`, `struct`) | Creates **separate** optimized native code for each → fast, no boxing |
| **Reference type** (`string`, classes) | Creates **one shared** native code for all (references are all the same size) |

**3. Static members belong to each closed type.**
Since `Counter<int>` and `Counter<string>` are really two different classes, each has its own statics:

```csharp
class Counter<T>
{
    public static int Count;
}

Counter<int>.Count++;
Counter<int>.Count++;
Counter<string>.Count++;

Console.WriteLine(Counter<int>.Count);      // 2
Console.WriteLine(Counter<string>.Count);   // 1   ← separate static field
```

> ✅ **Revise:** Reified (exists at runtime, unlike Java) · value types get own code, reference types share · statics are per closed type.

---

## 8. Generics vs `object` (boxing)

**Boxing** = wrapping a value type (like `int`) inside a heap object. **Unboxing** = taking it back out.

```csharp
// Non-generic: boxed on the way in, unboxed on the way out
ArrayList list = new ArrayList();
list.Add(5);                 // boxing (allocates an object on the heap)
int n = (int)list[0];        // unboxing + cast

// Generic: stored directly in an int[] inside, no boxing
List<int> list2 = new List<int>();
list2.Add(5);
int m = list2[0];
```

| | `ArrayList` / `object` | `List<T>` |
|---|---|---|
| Type safety | ❌ runtime errors | ✅ compile time |
| Boxing | Yes (slow, extra garbage) | None |
| Casting | Needed | Not needed |
| Use today? | Legacy, avoid | Default choice |

> ✅ **Revise:** Boxing = value type wrapped in a heap object (slow). Generics avoid it.

---

## 9. Generic collections

### Quick guide

| Type | Use it when you need |
|------|----------------------|
| `List<T>` | A resizable list with index access |
| `Dictionary<TKey, TValue>` | Fast lookup by key |
| `HashSet<T>` | Unique items, fast `Contains` |
| `Queue<T>` | First in, first out |
| `Stack<T>` | Last in, first out |
| `LinkedList<T>` | Fast insert/remove in the middle |
| `SortedDictionary<,>` / `SortedSet<T>` | Items kept sorted |
| `ConcurrentDictionary<,>` | Thread-safe use (`System.Collections.Concurrent`) |

**Interfaces:** `IEnumerable<T>` → `ICollection<T>` → `IList<T>`. Read-only: `IReadOnlyCollection<T>`, `IReadOnlyList<T>`.

| Interface | Gives you |
|-----------|-----------|
| `IEnumerable<T>` | Loop over items (forward only) |
| `ICollection<T>` | + `Count`, `Add`, `Remove` |
| `IList<T>` | + index access `list[i]` |
| `IReadOnlyList<T>` | Index access, **no** changes |

**Rule of thumb:** *accept* the most general type (`IEnumerable<T>`), *return* what best describes the result (`IReadOnlyList<T>`).

### How `List<T>` works (asked often)

`List<T>` wraps an **array**. When it's full, it creates a **bigger array** (double the size) and copies the items over.

```csharp
var list = new List<int>();
Console.WriteLine(list.Capacity);    // 0  (no array yet)

list.Add(1);
Console.WriteLine(list.Capacity);    // 4  (first array)

for (int i = 0; i < 4; i++) list.Add(i);   // now Count = 5
Console.WriteLine(list.Capacity);    // 8  (doubled)
```

| Term | Meaning |
|------|---------|
| `Count` | Items actually in the list |
| `Capacity` | Size of the internal array |

- Resizing copies every item (O(n)), but it happens rarely, so `Add` is **amortized O(1)**.
- If you know the size, give it up front to avoid resizing: `new List<int>(1000)`.
- Index access `list[i]` is O(1). `Insert` or `Remove` in the middle is O(n) (items shift).

### How `Dictionary<TKey, TValue>` works (asked often)

It's a **hash table**:

```
Add("apple", 5):
 1. key.GetHashCode()          → a number, e.g. 8734021
 2. hash % bucketCount         → bucket index, e.g. 3
 3. store the entry in that bucket

Lookup("apple"):
 1. compute the hash → find the bucket
 2. inside the bucket, compare keys with Equals() → return the value
```

- **Collision** = two different keys land in the same bucket. .NET chains the entries together and checks each with `Equals`.
- Average lookup / add / remove: **O(1)**. Worst case (many collisions): O(n).
- When it fills up, it **resizes** and re-distributes the entries.
- A key is found only if **both** `GetHashCode` and `Equals` agree. See the next section.

> ✅ **Revise:** `List<T>` = array that doubles (4 → 8 → 16), `Count` ≠ `Capacity`. `Dictionary` = hash table: hash → bucket → `Equals`, average O(1).

---

## 10. Equality: `IEquatable<T>`, `Equals`, `GetHashCode`

`Dictionary` and `HashSet` rely on **`GetHashCode()` and `Equals()`** of the key. For your own classes, define what "equal" means.

```csharp
public class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    // Type-safe version, no boxing, no cast
    public bool Equals(Money? other)
        => other is not null && Amount == other.Amount && Currency == other.Currency;

    // Keep the object versions consistent
    public override bool Equals(object? obj) => Equals(obj as Money);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
}

var a = new Money(10, "INR");
var b = new Money(10, "INR");
Console.WriteLine(a.Equals(b));                    // True (without overriding it would be False)

var set = new HashSet<Money> { a };
Console.WriteLine(set.Contains(b));                // True
```

### Why implement `IEquatable<T>`?
- `Equals(T)` is **type-safe** and avoids **boxing** for structs (`Dictionary`/`HashSet` use it through `EqualityComparer<T>.Default`).
- It's faster than `Equals(object)`.

### Rules
1. If two objects are **equal**, they **must have the same hash code**.
2. Override **both** `Equals` and `GetHashCode` together.
3. Use **immutable** fields for the hash code.

### Trap: changing a key after adding it

```csharp
class Key
{
    public int Id;
    public override int GetHashCode() => Id;
    public override bool Equals(object? o) => o is Key k && k.Id == Id;
}

var k = new Key { Id = 1 };
var dict = new Dictionary<Key, string> { [k] = "a" };

k.Id = 2;                                 // hash code changed after insertion
Console.WriteLine(dict.ContainsKey(k));   // False: the entry is "lost" in the wrong bucket
```

> ✅ **Revise:** Equal objects → same hash code. Override both. Implement `IEquatable<T>` for speed and no boxing. Never mutate a dictionary key. (`record` types do all this for you.)

---

## 11. Generic delegates

You rarely declare your own delegate types.

```csharp
Action<string> print = s => Console.WriteLine(s);       // input, no return
Func<int, int, int> add = (a, b) => a + b;              // last type = return type
Predicate<int> isEven = n => n % 2 == 0;                // input, returns bool

print("hi");
Console.WriteLine(add(2, 3));        // 5
Console.WriteLine(isEven(4));        // True
```

| Delegate | Meaning |
|----------|---------|
| `Action<T1,...>` | Returns **nothing** (`void`) |
| `Func<T1,...,TResult>` | Returns `TResult` (the **last** type parameter) |
| `Predicate<T>` | Takes `T`, returns `bool` |

LINQ is built on these: `Where(Func<T,bool>)`, `Select(Func<T,TResult>)`.

> ✅ **Revise:** `Action` = void, `Func` = returns last type, `Predicate` = bool.

---

## 12. Covariance and contravariance (simple version)

Use this class setup for all examples:

```csharp
class Animal { public string Name = ""; }
class Dog : Animal { }
class Cat : Animal { }
```

A `Dog` **is an** `Animal`, so this works for single objects:

```csharp
Animal a = new Dog();     // ✅
```

But what about **collections** of dogs?

### Step 1: Why `List<Dog>` is NOT a `List<Animal>` (invariance)

```csharp
List<Dog> dogs = new List<Dog>();
List<Animal> animals = dogs;       // ❌ compile error
```

If this were allowed:

```csharp
animals.Add(new Cat());            // legal for List<Animal>...
// ...but the real list holds Dogs, so now a Cat is inside a List<Dog> 💥
```

`List<T>` **reads and writes** `T`, so it is **invariant** (no conversion allowed).

### Step 2: Covariance (`out`): read-only things can "go wider"

```csharp
IEnumerable<Dog> dogs = new List<Dog> { new Dog() };
IEnumerable<Animal> animals = dogs;      // ✅ allowed
```

Why it's safe: `IEnumerable<T>` only **gives out** items. A basket of dogs can safely be used as a basket of animals, because you can only take animals out.

```csharp
public interface IProducer<out T> { T Produce(); }     // T only comes OUT
```

### Step 3: Contravariance (`in`): consume-only things can "go narrower"

```csharp
Action<Animal> feedAnimal = a => Console.WriteLine("Feeding " + a.Name);
Action<Dog> feedDog = feedAnimal;        // ✅ allowed
feedDog(new Dog());
```

Why it's safe: something that can feed **any animal** can certainly feed a **dog**.

```csharp
public interface IConsumer<in T> { void Consume(T item); }     // T only goes IN
```

### Memory trick

| Keyword | Name | `T` is only... | Direction |
|---------|------|----------------|-----------|
| **`out`** | Covariant | **returned** (output) | `Dog` → `Animal` (wider) |
| **`in`** | Contravariant | **accepted** (input) | `Animal` → `Dog` (narrower) |

**Out = produces = covariant. In = consumes = contravariant.**

### Quick table: "Can I assign it?"

| Assignment | Works? | Why |
|------------|--------|-----|
| `IEnumerable<Animal> = IEnumerable<Dog>` | ✅ | Covariant (`out`) |
| `List<Animal> = List<Dog>` | ❌ | Invariant |
| `Func<Animal> = Func<Dog>` | ✅ | `TResult` is `out` |
| `Action<Dog> = Action<Animal>` | ✅ | `T` is `in` |
| `IEnumerable<object> = IEnumerable<int>` | ❌ | Value types are not allowed |

### Where you see it

| Covariant (`out`) | Contravariant (`in`) |
|-------------------|----------------------|
| `IEnumerable<out T>` | `Action<in T>` |
| `IEnumerator<out T>` | `IComparer<in T>` |
| `IReadOnlyList<out T>` | `IComparable<in T>` |
| `Func<out TResult>` | `IEqualityComparer<in T>` |
| `Func<in T, out TResult>` has both | |

### Rules and traps

- Variance works **only on interfaces and delegates**, never on classes or structs.
- Works **only with reference types** (`int` → `object` needs boxing).
- `out T` can't be a method **parameter**. `in T` can't be **returned**.
- `List<T>` and `IList<T>` are **invariant** (they read and write).
- **Arrays are covariant (a legacy flaw):**

```csharp
object[] arr = new string[2];
arr[0] = 123;        // compiles, but 💥 ArrayTypeMismatchException at runtime
```

> ✅ **Revise:** `out` = covariant = read-only, `IEnumerable<Dog>` → `IEnumerable<Animal>`. `in` = contravariant = write-only, `Action<Animal>` → `Action<Dog>`. `List<T>` is invariant. Only interfaces and delegates, only reference types. Arrays are covariant and unsafe.

---

## 13. `IEnumerable<T>` vs `IQueryable<T>` (favourite question)

| | `IEnumerable<T>` | `IQueryable<T>` |
|---|------------------|-----------------|
| Where it runs | **In memory** (your app) | **In the data source** (e.g. the database) |
| Takes | `Func<T, bool>` (a delegate) | `Expression<Func<T, bool>>` (an expression tree) |
| What happens | Loops over objects already loaded | A provider (EF Core) **translates** the query to SQL |
| Use for | Collections in memory (`List`, arrays) | EF Core / database queries |

`IQueryable<T>` inherits from `IEnumerable<T>`.

```csharp
// IQueryable: filtering happens in SQL → only matching rows come back
var a = db.Orders.Where(o => o.Total > 100).ToList();
// SQL: SELECT ... FROM Orders WHERE Total > 100

// IEnumerable: ALL rows are loaded first, then filtered in memory 😬
var b = db.Orders.AsEnumerable().Where(o => o.Total > 100).ToList();
// SQL: SELECT ... FROM Orders    (no WHERE!)
```

### Traps
- Calling `.ToList()` or `.AsEnumerable()` **before** `Where` loads the whole table into memory.
- Both use **deferred execution**: the query runs only when you enumerate it (`foreach`, `ToList()`, `Count()`...).
- Returning `IQueryable<T>` from a repository **leaks** your data layer: callers can add more queries and the query may run after the `DbContext` is disposed.

> ✅ **Revise:** `IEnumerable` = in memory (delegates). `IQueryable` = translated to SQL (expression trees). Filter **before** `ToList()`.

---

## 14. Generics in real frameworks (and DI)

You use generics all the time without noticing:

| Where | Generic type | Why |
|-------|--------------|-----|
| Logging | `ILogger<OrderService>` | `T` sets the log **category** name |
| Configuration | `IOptions<AppSettings>` | Strongly typed settings |
| EF Core | `DbSet<Order>`, `DbContext.Set<T>()` | Typed tables |
| Async | `Task<T>`, `ValueTask<T>` | Async result of type `T` |
| Web API | `ActionResult<T>` | Typed responses |
| MediatR | `IRequestHandler<TRequest, TResponse>` | One handler per request type |
| Nullable | `int?` = `Nullable<int>` | Nullable value types |
| Laziness | `Lazy<T>` | Create on first use |
| Memory | `Span<T>` | Slices of memory without copying |

### Open generics in dependency injection

Register the **open** type once, and the container builds closed versions on demand.

```csharp
// Program.cs
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));   // open generic registration

public class OrderService(IRepository<Order> orders) { }    // container creates Repository<Order>
public class UserService(IRepository<User> users) { }       // container creates Repository<User>
```

- Note `typeof(IRepository<>)` has **empty angle brackets**.
- A specific registration, e.g. `IRepository<Order>` → `OrderRepository`, **takes precedence** over the open generic one.
- Same idea for MediatR pipeline behaviours (from your microservices course):

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

> ✅ **Revise:** Know where you've used generics (`ILogger<T>`, `Task<T>`, `DbSet<T>`, MediatR). Open-generic DI: `AddScoped(typeof(IRepo<>), typeof(Repo<>))`.

---

## 15. Practical example: `Result<T>`

For **expected** failures, return a result instead of throwing (ties to the Exception Handling notes).

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}

// Usage
Result<Order> r = orderService.Find(id);
if (!r.IsSuccess) return NotFound(r.Error);
return Ok(r.Value);
```

> ✅ **Revise:** `Result<T>` = success/failure object for expected failures. Exceptions are for the unexpected.

---

## 16. Practice exercise: build your own `MyStack<T>`

A classic interview coding task. It uses an array, resizing, and the Try pattern.

```csharp
public class MyStack<T>
{
    private T[] _items = new T[4];
    private int _count;

    public int Count => _count;

    public void Push(T item)
    {
        if (_count == _items.Length)
            Array.Resize(ref _items, _items.Length * 2);   // double when full, like List<T>

        _items[_count++] = item;
    }

    public T Pop()
    {
        if (_count == 0)
            throw new InvalidOperationException("Stack is empty");

        T item = _items[--_count];
        _items[_count] = default!;       // clear the slot so the GC can collect the object
        return item;
    }

    public T Peek()
    {
        if (_count == 0)
            throw new InvalidOperationException("Stack is empty");

        return _items[_count - 1];
    }

    public bool TryPop(out T item)       // Try pattern: no exception for an expected case
    {
        if (_count == 0)
        {
            item = default!;
            return false;
        }

        item = Pop();
        return true;
    }
}

// Usage
var stack = new MyStack<int>();
stack.Push(1);
stack.Push(2);
Console.WriteLine(stack.Pop());          // 2
Console.WriteLine(stack.Pop());          // 1
Console.WriteLine(stack.TryPop(out _));  // False
```

Points to mention in an interview: **LIFO**, array doubling (amortized O(1) push), `InvalidOperationException` for misuse vs `TryPop` for the expected case, clearing the slot for GC, and that it's **not thread-safe** (use `ConcurrentStack<T>` or a lock).

**Try yourself:** write `MyQueue<T>` (a circular array) or a generic `Cache<TKey, TValue>` with a size limit.

> ✅ **Revise:** Array + count, double when full, `default!` on pop, `Try...` method for expected failure.

---

## 17. Limitations of generics

- No `new T(args)`. Only `new T()` with the `new()` constraint.
- No operators (`+`, `>`) on `T` unless constrained (`INumber<T>`, `IComparable<T>`).
- Constructors, properties, indexers, events and finalizers **can't have their own type parameters** (they can use the class's `T`). Only classes, structs, interfaces, delegates and **methods** can be generic.
- `class A<T> : T` is not allowed.
- Two overloads can't differ only by constraints.
- Static members are **per closed type**.

---

## 18. Interview traps: quick list

1. Generics vs `object`: type safety, no boxing, no casting.
2. `new()` must be **last**, and only a **parameterless** constructor.
3. Generics are **reified** in .NET (kept at runtime), unlike Java erasure.
4. **Static fields are per closed type** (`Counter<int>` ≠ `Counter<string>`).
5. `List<Dog>` → `List<Animal>` ❌ (invariant). `IEnumerable<Dog>` → `IEnumerable<Animal>` ✅ (covariant).
6. Variance: **interfaces and delegates only**, **reference types only**.
7. **Arrays are covariant** and unsafe (`ArrayTypeMismatchException`).
8. `out` = output = covariant. `in` = input = contravariant.
9. No `==` or `+` on an unconstrained `T`: use `EqualityComparer<T>.Default` or `INumber<T>`.
10. `typeof(List<int>) != typeof(List<string>)`.
11. `default` = `0` / `false` / `null`.
12. `int?` is `Nullable<int>`, a generic struct.
13. Type inference works from **arguments**, not return types.
14. `List<T>` doubles its capacity (4 → 8 → 16). `Count` ≠ `Capacity`.
15. Equal objects must have the **same hash code**. Don't mutate dictionary keys.
16. `IQueryable` translates to SQL. `ToList()`/`AsEnumerable()` before `Where` loads everything.
17. Open-generic DI: `typeof(IRepository<>)` with empty brackets.

---

## 19. 30-second answers (for speaking in interviews)

**What are generics?**
"Generics let me write a class or method once with a type placeholder and pick the real type at use. I get compile-time type safety, no casting, no boxing for value types, and less duplicate code. `List<T>` is the classic example."

**Why generics over `object`?**
"`object` needs casts and boxes value types, and errors only show up at runtime. Generics are checked at compile time and value types are stored directly."

**What are constraints?**
"Constraints use `where` to tell the compiler what `T` supports, such as `class`, `struct`, `new()`, a base class, or an interface, so I can use those members inside the generic code. `new()` must come last."

**How do generics work at runtime?**
"In .NET they're reified: `List<int>` and `List<string>` are distinct runtime types. The JIT makes separate code for each value type and shares one for all reference types. Static fields are per closed type."

**Covariance vs contravariance?**
"Covariance (`out`) lets a read-only type like `IEnumerable<Dog>` act as `IEnumerable<Animal>`. Contravariance (`in`) lets a consumer like `Action<Animal>` act as `Action<Dog>`. It works only on interfaces and delegates with reference types. `List<T>` is invariant because it reads and writes."

**`IEnumerable` vs `IQueryable`?**
"`IEnumerable` runs in memory using delegates. `IQueryable` builds an expression tree that EF Core translates to SQL, so filtering happens in the database. I always filter before `ToList()`."

**How does `List<T>` work?**
"It wraps an array. When full it allocates one twice as big and copies the items, so `Add` is amortized O(1). Capacity is the array size, count is the items stored."

**How does `Dictionary` work?**
"It's a hash table: `GetHashCode` picks a bucket, collisions are chained, and `Equals` confirms the key. Average lookup is O(1). That's why `Equals` and `GetHashCode` must agree and keys shouldn't change."

---

## 20. Quick cheat sheet

| Concept | One-liner |
|---------|-----------|
| Generic class / method | Write once with `T`, use with any type |
| Open vs closed | `List<>` vs `List<int>` |
| Constraint | `where T : ...` tells the compiler what `T` can do |
| `new()` | Parameterless constructor, always last |
| `default` | `0` / `false` / `null` depending on `T` |
| Reified | Types exist at runtime in .NET |
| Value type instance | Own native code, no boxing |
| Reference type instance | Shared native code |
| Static members | Per closed type |
| `List<T>` | Array that doubles, `Count` vs `Capacity` |
| `Dictionary` | Hash table: hash → bucket → `Equals` |
| `IEquatable<T>` | Type-safe equality, no boxing |
| Covariance `out` | Read-only: `IEnumerable<Dog>` → `IEnumerable<Animal>` |
| Contravariance `in` | Consume-only: `Action<Animal>` → `Action<Dog>` |
| Invariant | `List<T>`, `IList<T>` |
| `IEnumerable` vs `IQueryable` | In memory vs translated to SQL |
| Open-generic DI | `AddScoped(typeof(IRepo<>), typeof(Repo<>))` |
| `Func` / `Action` / `Predicate` | Built-in generic delegates |
