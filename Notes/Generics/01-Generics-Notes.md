# Generics in C# (Simple Notes)

> **Definition:** Generics let you define classes, structs, interfaces, methods and delegates with a **type parameter** (a placeholder such as `T`). The real type is supplied when you use them. They give **compile-time type safety, no casting, no boxing** for value types, and **code reuse**.

```csharp
List<int> numbers = new List<int>();        // T = int
List<string> names = new List<string>();    // T = string
```

---

## 1. Why generics? (the problem they solve)

Before generics, collections stored `object`, so anything could go in and every read needed a cast.

```csharp
// Without generics
ArrayList list = new ArrayList();
list.Add(10);
list.Add("hello");              // compiles: any type is allowed
int x = (int)list[1];           // 💥 InvalidCastException at RUNTIME

// With generics
List<int> list2 = new List<int>();
list2.Add(10);
list2.Add("hello");             // ❌ compile error
int y = list2[0];               // no cast, no boxing
```

| Benefit | Meaning |
|---------|---------|
| **Type safety** | Mistakes are caught at compile time |
| **No casting** | No `(int)list[0]` |
| **No boxing** | Value types are stored directly (faster) |
| **Code reuse** | One `List<T>` instead of `IntList`, `StringList`, `OrderList` |

> **Interview trap:** "Generics vs `object` / `ArrayList`?" Answer with all four benefits.

---

## 2. Generic class

> **Definition:** A **generic class** is declared with one or more **type parameters** (`T`, `TKey`, `TValue`). The **type argument** is the real type you supply (`int` in `Box<int>`).
> **Open generic type** = type arguments not supplied (`List<>`). **Closed (constructed) type** = all supplied (`List<int>`). Only closed types can be instantiated.

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

Several type parameters:

```csharp
public class Pair<TKey, TValue>
{
    public TKey Key { get; }
    public TValue Value { get; }
    public Pair(TKey key, TValue value) { Key = key; Value = value; }
}

var p = new Pair<string, int>("age", 30);
```

**Naming:** `T` for a single parameter. For several, use descriptive names with a `T` prefix (`TKey`, `TValue`, `TEntity`, `TResult`).

---

## 3. Generic method

> **Definition:** A **generic method** has its own type parameters. It can live in a normal (non-generic) class.
> **Type inference:** the compiler works out `T` from the **method arguments**. It **cannot** infer `T` from the return type alone.

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
Swap(ref s1, ref s2);        // same method, T = string
```

```csharp
static T Create<T>() where T : new() => new T();

var order = Create<Order>();     // must be explicit: T can't be inferred from the return type
```

---

## 4. Generic interface and inheritance

> **Definition:** An interface or base class can also be generic. The implementing/derived type either **fixes** `T` (closed) or **passes it along** (still open).

```csharp
public interface IRepository<T>
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T item);
}

public class OrderRepository : IRepository<Order> { }             // fixes T = Order
public class InMemoryRepository<T> : IRepository<T> { }           // passes T along

public class Repository<T> { }
public class CachedRepository<T> : Repository<T> { }              // open: passes T
public class UserRepository : Repository<User> { }                // closed: fixes T
```

Inside `IRepository<T>` the compiler doesn't know `T` has an `Id`. That's what **constraints** solve.

---

## 5. Constraints (`where`)

> **Definition:** A **constraint** is a `where` clause that restricts which types can be used as `T`, and lets the generic code use the members those types are guaranteed to have.

Inside generic code, the compiler only knows `T` is an `object`:

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
| `where T : IInterface` | Implements the interface |
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
- Only a **parameterless** constructor works. `new T(args)` is **not allowed**. Use a factory (`Func<T>`) or `Activator.CreateInstance(typeof(T), args)`.

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

    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);   // Id known thanks to IEntity

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

Before .NET 7 you had to pass a `Func<T,T,T>` or use `dynamic` (slow).

---

## 6. `default` and comparing `T`

> **Definition:** `default` returns the default value of `T`: `null` for reference types, `0` for numbers, `false` for `bool`, and zeroed fields for structs.

```csharp
T value = default;
```

| `T` is... | `default` gives |
|-----------|-----------------|
| `int` | `0` |
| `bool` | `false` |
| `string`, any class | `null` |
| `DateTime` | `DateTime.MinValue` |
| a struct | all fields set to default |

Comparing two `T` values:

```csharp
if (a == b) { }                                  // ❌ not allowed for unconstrained T
EqualityComparer<T>.Default.Equals(a, b);        // ✅ correct way
if (value == null) { }                           // allowed (always false for non-nullable value types)
```

---

## 7. Boxing and unboxing

> **Definition:** **Boxing** wraps a value type (like `int`) in a heap-allocated `object`. **Unboxing** extracts it again with a cast. Both are slow and create garbage for the GC. Generic collections avoid them.

```csharp
// Non-generic: boxed on the way in, unboxed on the way out
ArrayList list = new ArrayList();
list.Add(5);                 // boxing
int n = (int)list[0];        // unboxing + cast

// Generic: stored directly inside an int[]
List<int> list2 = new List<int>();
list2.Add(5);
int m = list2[0];
```

| | `ArrayList` / `object` | `List<T>` |
|---|------------------------|-----------|
| Type safety | ❌ runtime errors | ✅ compile time |
| Boxing | Yes | None |
| Casting | Needed | Not needed |
| Use today? | Legacy, avoid | Default choice |

---

## 8. How generics work at runtime

> **Reified generics:** .NET keeps generic type information **at runtime**, so `List<int>` and `List<string>` are different types. Java uses **type erasure** instead (the type is removed at compile time).

```csharp
typeof(List<int>) == typeof(List<string>);   // false
typeof(List<>);                              // open generic type
typeof(List<int>).Name;                      // "List`1"  (1 = number of type parameters)
```

> **JIT specialization:** For each **value-type** argument (`int`, a struct) the JIT generates **separate native code**: fast, no boxing. For **reference-type** arguments (`string`, classes) it generates **one shared code** (all references are the same size).

> **Per-type statics:** each closed generic type has its **own static members**.

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

---

## 9. Generic collections

> **Definition:** The generic collections in `System.Collections.Generic` are type-safe, boxing-free replacements for the old `System.Collections` ones (`ArrayList`, `Hashtable`).

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

### Collection interfaces

| Interface | Gives you |
|-----------|-----------|
| `IEnumerable<T>` | Loop over items (forward only) |
| `ICollection<T>` | + `Count`, `Add`, `Remove` |
| `IList<T>` | + index access `list[i]` |
| `IReadOnlyList<T>` | Index access, **no** changes |

Hierarchy: `IEnumerable<T>` → `ICollection<T>` → `IList<T>`.
**Rule of thumb:** *accept* the most general type (`IEnumerable<T>`), *return* what best describes the result (`IReadOnlyList<T>` or `List<T>`).

### How `List<T>` works (asked often)

> **Definition:** `List<T>` is a resizable list backed by an **array**. When the array is full, it allocates a new one **twice the size** and copies the items. `Count` = items stored. `Capacity` = size of the internal array.

```csharp
var list = new List<int>();
Console.WriteLine(list.Capacity);    // 0  (no array yet)

list.Add(1);
Console.WriteLine(list.Capacity);    // 4  (first array)

for (int i = 0; i < 4; i++) list.Add(i);   // Count = 5
Console.WriteLine(list.Capacity);    // 8  (doubled)
```

- A resize copies everything (O(n)), but it happens rarely, so `Add` is **amortized O(1)**.
- If you know the size, pass it in: `new List<int>(1000)`.
- Index access is O(1). `Insert` / `Remove` in the middle is O(n) (items shift).

### How `Dictionary<TKey, TValue>` works (asked often)

> **Definition:** `Dictionary` is a **hash table**. The key's hash code picks a **bucket**, and `Equals` confirms the match inside it. Average lookup, add and remove are **O(1)**.

```
Add("apple", 5):
 1. key.GetHashCode()      → a number
 2. hash % bucketCount     → bucket index
 3. store the entry in that bucket

Lookup("apple"):
 1. compute the hash → find the bucket
 2. compare keys in the bucket with Equals() → return the value
```

- **Collision:** two different keys land in the same bucket. .NET chains the entries and checks each with `Equals`. Worst case O(n).
- It **resizes** when full and redistributes entries.
- `HashSet<T>` uses the same hashing idea for unique items.
- A key is found only if **both** `GetHashCode` and `Equals` agree (see next section).

---

## 10. Equality: `IEquatable<T>`, `Equals`, `GetHashCode`

> **Definition:** `IEquatable<T>` is an interface with a **type-safe** `bool Equals(T other)`. Collections use it (through `EqualityComparer<T>.Default`) to compare items **without boxing**. It's faster than `Equals(object)`.

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

    public bool Equals(Money? other)
        => other is not null && Amount == other.Amount && Currency == other.Currency;

    public override bool Equals(object? obj) => Equals(obj as Money);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
}

var a = new Money(10, "INR");
var b = new Money(10, "INR");
Console.WriteLine(a.Equals(b));                  // True (without the override it would be False)

var set = new HashSet<Money> { a };
Console.WriteLine(set.Contains(b));              // True
```

### Rules
1. If two objects are **equal**, they **must have the same hash code**.
2. Override **both** `Equals` and `GetHashCode` together.
3. Base the hash code on **immutable** fields.
4. `record` types generate all of this automatically.

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
Console.WriteLine(dict.ContainsKey(k));   // False: the entry is lost in the wrong bucket
```

---

## 11. Generic delegates and `Nullable<T>`

> **Definition:** The built-in generic delegates mean you rarely declare your own delegate types.
> - `Action<T1,...>`: returns **nothing**
> - `Func<T1,...,TResult>`: returns a value (the **last** type parameter)
> - `Predicate<T>`: takes `T`, returns `bool`

```csharp
Action<string> print = s => Console.WriteLine(s);
Func<int, int, int> add = (a, b) => a + b;
Predicate<int> isEven = n => n % 2 == 0;

print("hi");
Console.WriteLine(add(2, 3));       // 5
Console.WriteLine(isEven(4));       // True
```

LINQ is built on these: `Where(Func<T,bool>)`, `Select(Func<T,TResult>)`.

> **`Nullable<T>`** is a generic struct (`where T : struct`) that lets a value type hold `null`. `int?` is shorthand for `Nullable<int>`.

```csharp
int? age = null;
Console.WriteLine(age.HasValue);     // False
age = 30;
Console.WriteLine(age.Value);        // 30
```

---

## 12. Covariance and contravariance

> **Invariance:** no conversion between different type arguments. `List<Dog>` is **not** a `List<Animal>`.
> **Covariance (`out`):** a type of a **more derived** `T` can be used where a **less derived** `T` is expected (`IEnumerable<Dog>` → `IEnumerable<Animal>`). `T` may only be **returned** (output).
> **Contravariance (`in`):** the reverse (`Action<Animal>` → `Action<Dog>`). `T` may only be **accepted** (input).

Setup used below:

```csharp
class Animal { public string Name = ""; }
class Dog : Animal { }
class Cat : Animal { }
```

### Step 1: why `List<Dog>` is NOT a `List<Animal>` (invariance)

```csharp
List<Dog> dogs = new List<Dog>();
List<Animal> animals = dogs;       // ❌ compile error
```

If it were allowed, `animals.Add(new Cat())` would put a `Cat` inside a list of dogs 💥. `List<T>` both reads and writes `T`, so it is invariant.

### Step 2: covariance (`out`): read-only types can "go wider"

```csharp
IEnumerable<Dog> dogs = new List<Dog> { new Dog() };
IEnumerable<Animal> animals = dogs;      // ✅
```

Safe because `IEnumerable<T>` only **gives out** items: a sequence of dogs is a sequence of animals.

```csharp
public interface IProducer<out T> { T Produce(); }     // T only comes OUT
```

### Step 3: contravariance (`in`): consume-only types can "go narrower"

```csharp
Action<Animal> feedAnimal = a => Console.WriteLine("Feeding " + a.Name);
Action<Dog> feedDog = feedAnimal;        // ✅
feedDog(new Dog());
```

Safe because something that can handle **any animal** can handle a dog.

```csharp
public interface IConsumer<in T> { void Consume(T item); }     // T only goes IN
```

### Memory trick

**`out` = produces = covariant.  `in` = consumes = contravariant.**

| Assignment | Works? | Why |
|------------|--------|-----|
| `IEnumerable<Animal> = IEnumerable<Dog>` | ✅ | Covariant (`out`) |
| `List<Animal> = List<Dog>` | ❌ | Invariant |
| `Func<Animal> = Func<Dog>` | ✅ | `TResult` is `out` |
| `Action<Dog> = Action<Animal>` | ✅ | `T` is `in` |
| `IEnumerable<object> = IEnumerable<int>` | ❌ | Value types aren't allowed |

| Covariant (`out`) | Contravariant (`in`) |
|-------------------|----------------------|
| `IEnumerable<out T>` | `Action<in T>` |
| `IEnumerator<out T>` | `IComparer<in T>` |
| `IReadOnlyList<out T>` | `IComparable<in T>` |
| `Func<out TResult>` | `IEqualityComparer<in T>` |

### Rules and traps

- Works **only on interfaces and delegates**, never on classes or structs.
- Works **only with reference types** (`int` → `object` would need boxing).
- `out T` can't be a method **parameter**. `in T` can't be **returned**.
- **Arrays are covariant (a legacy flaw):**

```csharp
object[] arr = new string[2];
arr[0] = 123;        // compiles, but 💥 ArrayTypeMismatchException at runtime
```

---

## 13. `IEnumerable<T>` vs `IQueryable<T>` (favourite question)

> **`IEnumerable<T>`:** forward-only iteration over a sequence, **in memory**, using delegates (`Func<T,bool>`).
> **`IQueryable<T>`:** a query held as an **expression tree** (`Expression<Func<T,bool>>`) that a provider such as EF Core **translates to SQL**, so filtering happens in the database. It inherits from `IEnumerable<T>`.

```csharp
// IQueryable: filtering happens in SQL, so only matching rows come back
var a = db.Orders.Where(o => o.Total > 100).ToList();
// SQL: SELECT ... FROM Orders WHERE Total > 100

// IEnumerable: ALL rows are loaded first, then filtered in memory
var b = db.Orders.AsEnumerable().Where(o => o.Total > 100).ToList();
// SQL: SELECT ... FROM Orders    (no WHERE)
```

### Traps
- Calling `.ToList()` or `.AsEnumerable()` **before** `Where` loads the whole table into memory.
- Both use **deferred execution**: the query runs only when you enumerate it (`foreach`, `ToList()`, `Count()`).
- Returning `IQueryable<T>` from a repository **leaks** the data layer, and the query may run after the `DbContext` is disposed.

---

## 14. Generics in DI and frameworks

> **Open-generic registration:** registering `typeof(IRepository<>)` → `typeof(Repository<>)` once, so the DI container creates closed versions (`IRepository<Order>`) on demand.

```csharp
// Program.cs
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));   // note the EMPTY brackets

public class OrderService(IRepository<Order> orders) { }    // container creates Repository<Order>
public class UserService(IRepository<User> users) { }       // container creates Repository<User>
```

- A **specific** registration (`IRepository<Order>` → `OrderRepository`) takes **precedence** over the open generic one.
- Same idea for MediatR pipeline behaviours:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

### Generics you already use

| Where | Generic type |
|-------|--------------|
| Logging | `ILogger<OrderService>` (`T` sets the log category) |
| Configuration | `IOptions<AppSettings>` |
| EF Core | `DbSet<Order>`, `Set<T>()` |
| Async | `Task<T>`, `ValueTask<T>` |
| Web API | `ActionResult<T>` |
| MediatR | `IRequestHandler<TRequest, TResponse>` |
| Others | `Nullable<T>`, `Lazy<T>`, `Span<T>` |

---

## 15. Limitations of generics

- No `new T(args)`. Only `new T()` with the `new()` constraint.
- No operators (`+`, `>`) on `T` unless constrained (`INumber<T>`, `IComparable<T>`).
- Constructors, properties, indexers, events and finalizers **can't have their own type parameters** (they can use the class's `T`). Classes, structs, interfaces, delegates and **methods** can be generic.
- `class A<T> : T` is not allowed.
- Two overloads can't differ only by constraints.
- Static members are **per closed type**.

---

## 16. Interview traps: quick list

1. Generics vs `object`: type safety, no boxing, no casting.
2. `new()` must be **last**, and only a **parameterless** constructor.
3. Generics are **reified** in .NET (kept at runtime), unlike Java erasure.
4. **Static fields are per closed type**: `Counter<int>` ≠ `Counter<string>`.
5. `List<Dog>` → `List<Animal>` ❌ (invariant). `IEnumerable<Dog>` → `IEnumerable<Animal>` ✅ (covariant).
6. Variance: **interfaces and delegates only**, **reference types only**.
7. **Arrays are covariant** and unsafe (`ArrayTypeMismatchException`).
8. `out` = output = covariant. `in` = input = contravariant.
9. No `==` or `+` on an unconstrained `T`: use `EqualityComparer<T>.Default` or `INumber<T>`.
10. `default` = `0` / `false` / `null`.
11. `int?` is `Nullable<int>`, a generic struct.
12. Type inference works from **arguments**, not from the return type.
13. `List<T>` doubles its capacity (4 → 8 → 16). `Count` ≠ `Capacity`.
14. Equal objects must have the **same hash code**. Never mutate a dictionary key.
15. `IQueryable` translates to SQL. `ToList()` / `AsEnumerable()` before `Where` loads everything.
16. Open-generic DI uses `typeof(IRepository<>)` with empty brackets.

---

## Quick cheat sheet

| Concept | One-liner |
|---------|-----------|
| Generic class / method | Write once with `T`, use with any type |
| Open vs closed | `List<>` vs `List<int>` |
| Constraint | `where T : ...` tells the compiler what `T` can do |
| `new()` | Parameterless constructor, always last |
| `default` | `0` / `false` / `null` depending on `T` |
| Boxing | Value type wrapped in a heap object (slow); generics avoid it |
| Reified | Generic types exist at runtime in .NET |
| Value-type argument | Own native code, no boxing |
| Reference-type argument | Shared native code |
| Statics | Per closed type |
| `List<T>` | Array that doubles, `Count` vs `Capacity` |
| `Dictionary` | Hash table: hash → bucket → `Equals` |
| `IEquatable<T>` | Type-safe equality, no boxing |
| Covariance `out` | `IEnumerable<Dog>` → `IEnumerable<Animal>` |
| Contravariance `in` | `Action<Animal>` → `Action<Dog>` |
| Invariant | `List<T>`, `IList<T>` |
| `IEnumerable` vs `IQueryable` | In memory vs translated to SQL |
| Open-generic DI | `AddScoped(typeof(IRepo<>), typeof(Repo<>))` |
| `Func` / `Action` / `Predicate` | Built-in generic delegates |
