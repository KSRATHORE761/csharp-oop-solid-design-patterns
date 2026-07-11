# Interfaces - Part 2

---

# 14. Interface Inheritance

An interface can inherit from one or more interfaces.

## Example

```csharp
public interface IVehicle
{
    void Start();
}

public interface IChargeable : IVehicle
{
    void ChargeBattery();
}

public class ElectricCar : IChargeable
{
    public void Start()
    {
        Console.WriteLine("Starting...");
    }

    public void ChargeBattery()
    {
        Console.WriteLine("Charging...");
    }
}
```

---

# 15. Explicit Interface Implementation

Use explicit implementation when:

- Two interfaces have methods with the same signature.
- You want the method accessible only through the interface reference.

## Example

```csharp
public interface IPrinter
{
    void Print();
}

public interface IScanner
{
    void Print();
}

public class MultiFunctionMachine : IPrinter, IScanner
{
    void IPrinter.Print()
    {
        Console.WriteLine("Printer");
    }

    void IScanner.Print()
    {
        Console.WriteLine("Scanner");
    }
}
```

Usage

```csharp
IPrinter printer = new MultiFunctionMachine();
printer.Print();

IScanner scanner = new MultiFunctionMachine();
scanner.Print();
```

---

# 16. Default Interface Methods (C# 8.0+)

Interfaces can provide default implementations.

## Example

```csharp
public interface ILogger
{
    void Log(string message)
    {
        Console.WriteLine(message);
    }
}
```

> Rarely used in day-to-day development but useful for backward compatibility.

---

# 17. When Should You Use an Interface?

Use an interface when:

- Multiple unrelated classes need the same behavior.
- You need loose coupling.
- You are using Dependency Injection.
- You want to mock dependencies in unit tests.
- You want multiple inheritance of behavior.

---

# 18. When NOT to Use an Interface

Avoid interfaces when:

- Classes share common state.
- Classes share implementation.
- A base class can provide reusable code.

In these cases, prefer an **abstract class**.

---

# 19. Real-World Examples

## Payment System

```
IPayment

↓

CreditCardPayment

UPIPayment

PayPalPayment
```

---

## Notification Service

```
INotification

↓

EmailNotification

SMSNotification

PushNotification
```

---

## File Storage

```
IStorage

↓

AzureStorage

AWSStorage

LocalStorage
```

---

# 20. Advantages

- Loose coupling.
- High flexibility.
- Better testability.
- Supports Dependency Injection.
- Supports multiple interface implementation.
- Easier maintenance.

---

# 21. Limitations

- Cannot maintain object state.
- No constructors.
- No instance fields.
- Too many interfaces can make a design difficult to understand.

---

# 22. Interview Questions

## Q1. Can an interface inherit another interface?

**Answer**

Yes.

An interface can inherit one or more interfaces.

---

## Q2. Can a class inherit multiple interfaces?

**Answer**

Yes.

A class can implement multiple interfaces.

```csharp
class Car : IChargeable, IServiceable
{
}
```

---

## Q3. Can an interface contain constructors?

**Answer**

No.

Interfaces cannot have constructors because they cannot be instantiated.

---

## Q4. Can an interface contain fields?

**Answer**

No.

Interfaces cannot contain instance fields because they don't maintain object state.

---

## Q5. Can interfaces have static methods?

**Answer**

Yes (modern C# versions).

However, this is not commonly used in enterprise applications.

---

## Q6. What is Explicit Interface Implementation?

**Answer**

It allows implementing interface members so they are accessible only through the interface reference.

It is mainly used when multiple interfaces contain methods with the same signature.

---

## Q7. Why are interfaces important for Dependency Injection?

**Answer**

Dependency Injection depends on abstractions rather than concrete classes.

Interfaces allow implementations to be replaced without changing client code.

---

## Q8. Why are interfaces important for Unit Testing?

**Answer**

Interfaces allow dependencies to be mocked.

Example:

```csharp
IEmailService
```

can easily be replaced with a fake implementation during testing.

---

## Q9. Can an interface have private methods?

**Answer**

No (for interview purposes).

Traditional interfaces expose public contracts only.

---

## Q10. Can an interface have implemented methods?

**Answer**

Yes.

Since C# 8.0, interfaces can have default implementations.

---

# 23. Common Mistakes

❌ Using interfaces when classes already share common implementation.

✔ Use an abstract class instead.

---

❌ Creating an interface for every class.

✔ Create interfaces only when there is a business need for abstraction.

---

❌ Thinking interfaces can store object data.

✔ Interfaces define behavior, not state.

---

# 24. Best Practices

- Keep interfaces small (Interface Segregation Principle).
- Name interfaces with the `I` prefix (e.g., `ILogger`).
- Prefer interfaces for services and dependencies.
- Avoid "god interfaces" with too many methods.
- Design interfaces around business capabilities.

---

# 25. Interview Summary

## Definition

An interface defines a contract that implementing classes must follow.

---

## Keywords

- interface
- implements
- explicit implementation
- multiple interfaces
- Dependency Injection

---

## Remember

- Cannot instantiate.
- No constructors.
- No instance fields.
- Supports multiple implementation.
- Promotes loose coupling.
- Widely used in DI and Unit Testing.

---

# Quick Revision

- ✅ Definition
- ✅ Why Interfaces?
- ✅ Characteristics
- ✅ Interface Members
- ✅ Multiple Interface Implementation
- ✅ Interface Inheritance
- ✅ Explicit Interface Implementation
- ✅ Default Interface Methods
- ✅ Advantages
- ✅ Limitations
- ✅ Interview Questions
- ✅ Common Mistakes