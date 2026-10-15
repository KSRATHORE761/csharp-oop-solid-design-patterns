# Chapter: Properties in C#

- A property is a member of a class that provides a **flexible way to read, write or compute** the values of private fields.
- It acts as a **combination of a method and a variable**.

## Why use properties?

1. **Encapsulation & protection:** shields the class's private fields from direct access by other classes.
2. **Controlled access:** allows validation rules, data logging, or extra logic before a value is read or changed.

## Syntax

```csharp
<access_modifier> <return_type> <property_name>
{
    get { /* return value */ }
    set { /* assign value using keyword 'value' */ }
}
```

## Accessors

The `get` and `set` blocks are called **accessors**.

1. **get accessor**: returns the value
2. **set accessor**: assigns a value using the `value` keyword

### Example (with validation)

```csharp
private int age;

public int Age
{
    get { return age; }
    set
    {
        if (value >= 0)
        {
            age = value;
        }
        else
        {
            Console.WriteLine("Age cannot be negative");
        }
    }
}
```

> **Tip:** In real projects, throwing an exception (e.g. `ArgumentOutOfRangeException`) is usually better than printing to the console, because the caller can then handle the invalid value.

---

## Types of Properties

### 1. Read-Write property
Allows full reading and modification of the field.

```csharp
public int RollNo
{
    get { return rollNo; }
    set { rollNo = value; }
}
```

### 2. Read-only property
Contains **only the `get` accessor**.

```csharp
private int r = 357;

public int Roll_No
{
    get { return r; }
}
```

### 3. Write-only property
Contains **only the `set` accessor**.

### 4. Auto-implemented properties

```csharp
public string Name { get; set; }
```

When no additional logic is needed in the accessors, you can omit the explicit private backing field. The compiler automatically generates an **invisible private backing field** for you behind the scenes.

---

## Accessor Accessibility

We can add individual access modifiers (like `private` or `protected`) directly to individual `get` or `set` blocks, subject to these constraints:

1. **Both** `set` and `get` accessors are required (the property must have both to restrict one).
2. The accessor modifier must be **more restrictive** than the level of the property itself (e.g. a `private` property cannot have a `public set`).
3. **No modifiers on interfaces.**
4. If a property uses an `override` modifier, its accessor modifier must **exactly match** the accessor restriction of the base property being overridden.

```csharp
public string Name { get; private set; }   // readable by all, writable only inside the class
```

---

## Quick Recap

| Type | Accessors |
|------|-----------|
| Read-write | get + set |
| Read-only | get only |
| Write-only | set only |
| Auto-implemented | `{ get; set; }` with compiler-generated backing field |