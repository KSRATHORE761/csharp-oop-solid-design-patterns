# Chapter: Access Modifiers in C#

Access modifiers define the **scope and visibility** of classes, methods, fields, constructors and other members. They determine **where and how** a member can be accessed in the program.

| # | Modifier | Accessible from |
|---|----------|-----------------|
| 1 | `public` | Anywhere in the project or from other assemblies |
| 2 | `private` | Only within the same class. **Default** for class members if none is specified |
| 3 | `protected` | Within the same class and by derived classes |
| 4 | `internal` | Only within the same assembly (project). Not visible to external assemblies unless explicitly exposed with `InternalsVisibleTo` |
| 5 | `protected internal` | **Protected OR Internal**: same assembly, and also derived classes even if they are in a different assembly |
| 6 | `private protected` | **Private AND Protected**: only derived classes that reside within the same assembly (plus the containing class itself) |

## Quick memory trick

- `protected internal` = **OR** → wider access (either condition is enough)
- `private protected` = **AND** → narrower access (both conditions needed)

```csharp
public class Base
{
    public int A;                     // everywhere
    private int B;                    // this class only
    protected int C;                  // this class + derived classes
    internal int D;                   // same assembly
    protected internal int E;         // same assembly OR derived classes (any assembly)
    private protected int F;          // derived classes in same assembly only
}
```

---

## Critical Default Access Rules

- **Top-level types** (classes, structs, interfaces declared directly within a namespace) default to **`internal`**.
- **Class & struct members** (fields, methods, nested types) default to **`private`**.
- **Interface members** default to **`public`**.

## Key Rules & Restrictions

1. **Derived classes cannot have greater accessibility than their base types.**
2. **Struct limitations:** structs do not support inheritance, so we cannot declare struct members with `protected`, `protected internal`, or `private protected`.
3. **Namespaces** do not allow access modifiers. They are always public.

---

## Quick Recap

| Where | Default |
|-------|---------|
| Top-level class / struct / interface | `internal` |
| Class / struct members | `private` |
| Interface members | `public` |
| Namespace | always public (no modifier allowed) |