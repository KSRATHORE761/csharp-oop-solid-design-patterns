# Chapter 2: Constructors in C#

A constructor is a **special method** of a class that is automatically called when an object of the class is created.

- It has the **same name as the class** and has **no return type**.
- It is mainly used to **initialize the object's data members**.

```csharp
class Geeks
{
    public Geeks() { }                 // constructor
}

Geeks obj = new Geeks();               // calls the constructor
```

---

## Types of Constructors

### 1. Default Constructor

A constructor with **no parameters**. It is automatically provided by the compiler if no constructor is defined in the class. It initializes fields to their default values.

```csharp
class Geeks
{
    int num;
    string name;

    public Geeks()
    {
        Console.WriteLine("Constructor called");
    }

    public static void Main()
    {
        Geeks geek = new Geeks();
        Console.WriteLine("num = " + geek.num);
        Console.WriteLine("name = " + geek.name);
    }
}
```

**Output:**

```
Constructor called
num = 0
name =
```

> Default values: `0` for numeric types, `false` for bool, `null` for reference types (string, objects).
> Note: once you define *any* constructor yourself, the compiler no longer provides the default one.

---

### 2. Parameterized Constructor

A constructor having **at least one parameter**. It can initialize each instance of a class with different values.

```csharp
class Geek
{
    string n;       // store name
    int i;          // store id

    public Geek(string n, int i)
    {
        this.n = n;
        this.i = i;
    }

    public static void Main()
    {
        Geek geek = new Geek("Geek", 1);
        Console.WriteLine("Name = " + geek.n + " Id = " + geek.i);
    }
}
```

`this` refers to the current object. It separates the field (`this.n`) from the parameter (`n`).

---

### 3. Copy Constructor

Used to create a **new object by copying values from an existing object** of the same class. In C#, copy constructors must be **explicitly defined by the user** (there is no built-in one).

```csharp
class Geeks
{
    private string month;
    private int year;

    // Copy constructor
    public Geeks(Geeks s)
    {
        month = s.month;
        year = s.year;
    }

    // Instance constructor
    public Geeks(string month, int year)
    {
        this.month = month;
        this.year = year;
    }

    // Get details of Geeks
    public string Details
    {
        get
        {
            return "Month: " + month.ToString() + " Year: " + year.ToString();
        }
    }

    public static void Main()
    {
        // Create a new Geeks object
        Geeks g1 = new Geeks("June", 2018);

        // Here g1's details are copied to g2
        Geeks g2 = new Geeks(g1);
        Console.WriteLine(g2.Details);
    }
}
```

---

### 4. Private Constructor

Created using the `private` access specifier.

- Other classes **cannot create an instance** of this class from outside, and cannot derive from it.
- Often used in implementing the **Singleton pattern**.
- Use a private constructor when the class has **only static members**.

```csharp
public class Geeks
{
    private Geeks()
    {
        Console.WriteLine("from private constructor");
    }
}
```

---

### 5. Static Constructor

A static constructor is invoked **only once** in the class, during the creation of the **first instance** or the **first reference to a static member**. It is used to initialize **static fields/data** of a class.

Rules:

- Cannot be called directly.
- The user has **no control** over when it executes.
- Does **not** take access modifiers or parameters.

```csharp
class Geeks
{
    // Invoked before the first instance constructor runs
    static Geeks()
    {
        Console.WriteLine("Static constructor");
    }

    public Geeks(int i)
    {
        Console.WriteLine("Instance constructor " + i);
    }

    public string geeks_detail(string name, int id)
    {
        return "Name: " + name + " id: " + id;
    }

    public static void Main()
    {
        Geeks obj = new Geeks(1);
        Console.WriteLine(obj.geeks_detail("GFG", 1));

        Geeks obj1 = new Geeks(2);
        Console.WriteLine(obj1.geeks_detail("GFGs", 2));
    }
}
```

**Output:** (the static constructor prints only once)

```
Static constructor
Instance constructor 1
Name: GFG id: 1
Instance constructor 2
Name: GFGs id: 2
```

### Static members (related example)

```csharp
class Geeks
{
    public static int count_geeks;

    public static int geeks_count()
    {
        return ++count_geeks;
    }

    public static void Main()
    {
        // Static members belong to the class, so access them via the class name.
        // Accessing them through an instance (e.g. s.count_geeks) is a compile error.
        Geeks.count_geeks = 99;
        Geeks.geeks_count();
        Console.WriteLine(Geeks.count_geeks);   // 100
    }
}
```

---

# Destructors in C#

A destructor is a special method in a class that is automatically invoked by the **Garbage Collector** just before an object is destroyed or removed from memory.

### Syntax

```csharp
class ClassName
{
    // Destructor: declared using tilde (~)
    ~ClassName()
    {
        // Cleanup code for unmanaged resources
    }
}
```

**Primary purpose:** release **unmanaged resources** (database connections, file handles, etc.) before the object memory is reclaimed.

### Rules

- No access specifiers and no parameters.
- Exactly **one per class**.
- No overloading or inheritance.
- Cannot be defined inside **structs**.

### How it works

Internally, the C# compiler automatically translates a destructor to `Finalize()`.

### Example

```csharp
class Geeks
{
    Geeks()
    {
        Console.WriteLine("Object created");
    }

    ~Geeks()
    {
        Console.WriteLine("Object destroyed");
    }

    public void DisplayMessage()
    {
        Console.WriteLine("Message printed");
    }

    static void Main(string[] args)
    {
        Geeks geek = new Geeks();
        geek.DisplayMessage();
    }
}
```

**Output:**

```
Object created
Message printed
Object destroyed
```

> **Interview note:** You cannot predict *when* the destructor runs, because the GC decides. For deterministic cleanup, implement `IDisposable` and use a `using` block. Destructors are a safety net, not the primary mechanism.

---

## Quick Recap

| Constructor | Key point |
|-------------|-----------|
| Default | No parameters; compiler-provided if none defined |
| Parameterized | Takes parameters; different values per object |
| Copy | Copies from an existing object; user-defined |
| Private | Blocks outside instantiation; used in Singleton |
| Static | Runs once; initializes static data; no access modifier/parameters |