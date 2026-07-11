# Interfaces - Part 1

---

# 1. Definition

An **interface** defines a **contract** that implementing classes must follow.

It specifies **what** a class should do, but not **how** it should do it.

---

# 2. Why Do We Need Interfaces?

Interfaces help us:

- Achieve abstraction.
- Reduce coupling between classes.
- Support Dependency Injection (DI).
- Enable multiple inheritance of behavior.
- Improve testability.
- Make applications easier to extend.

---

# 3. Real-World Examples

## Payment Gateway

```
IPayment

    ↓

CreditCardPayment
UPIPayment
PayPalPayment
```

Every payment method follows the same contract:

- Pay()
- Refund()

Each implements it differently.

---

## Notification System

```
INotification

      ↓

EmailNotification
SMSNotification
PushNotification
```

Each notification type implements:

```
Send()
```

---

# 4. Interface Syntax

```csharp
public interface IVehicle
{
    void Start();
    void Stop();
}
```

Implementation

```csharp
public class Car : IVehicle
{
    public void Start()
    {
        Console.WriteLine("Car Started");
    }

    public void Stop()
    {
        Console.WriteLine("Car Stopped");
    }
}
```

---

# 5. Characteristics

- Cannot create objects.
- Contains method declarations.
- Can contain properties.
- Can contain events.
- Can contain indexers.
- A class can implement multiple interfaces.

---

# 6. Interface Members

An interface can contain:

- Methods
- Properties
- Events
- Indexers

Example

```csharp
public interface IVehicle
{
    void Start();

    string Brand { get; }

    event EventHandler Started;
}
```

---

# 7. Implementing an Interface

Example

```csharp
public interface IChargeable
{
    void ChargeBattery();
}

public class ElectricCar : IChargeable
{
    public void ChargeBattery()
    {
        Console.WriteLine("Charging...");
    }
}
```

---

# 8. Multiple Interface Implementation

A class can implement multiple interfaces.

Example

```csharp
public interface IChargeable
{
    void Charge();
}

public interface IServiceable
{
    void Service();
}

public class ElectricCar : IChargeable, IServiceable
{
    public void Charge()
    {
        Console.WriteLine("Charging");
    }

    public void Service()
    {
        Console.WriteLine("Servicing");
    }
}
```

---

# 9. Advantages

- Loose coupling.
- High flexibility.
- Better testability.
- Supports Dependency Injection.
- Multiple inheritance of behavior.
- Easy to extend.

---

# 10. Limitations

- Cannot store object state.
- Cannot have instance fields.
- Too many interfaces can increase complexity.

---

# 11. Interview Questions

## Q1. What is an interface?

**Answer**

An interface defines a contract that implementing classes must follow. It specifies what operations a class should perform without providing the implementation.

---

## Q2. Why do we use interfaces?

**Answer**

Interfaces provide:

- Abstraction
- Loose coupling
- Better maintainability
- Multiple inheritance of behavior
- Dependency Injection support

---

## Q3. Can we create an object of an interface?

**Answer**

No.

Interfaces cannot be instantiated because they only define a contract.

---

## Q4. Can a class implement multiple interfaces?

**Answer**

Yes.

This is one of the biggest advantages of interfaces.

Example

```csharp
class ElectricCar : IChargeable, IServiceable
{
}
```

---

## Q5. Can an interface contain fields?

**Answer**

No.

Interfaces cannot contain instance fields because they do not store object state.

---

## Q6. Can an interface contain properties?

**Answer**

Yes.

Example

```csharp
public interface IVehicle
{
    string Brand { get; }
}
```

---

# 12. Common Mistakes

❌ Using interfaces to share common implementation.

✔ Use an abstract class when implementation needs to be shared.

---

❌ Creating one interface per class without a business need.

✔ Create interfaces for shared capabilities.

---

❌ Thinking interfaces can contain instance fields.

✔ Interfaces define behavior, not state.

---

# 13. Quick Revision

## Definition

Defines a contract.

---

## Keywords

- interface
- implements

---

## Remember

- Cannot instantiate.
- No instance fields.
- Supports multiple implementation.
- Improves loose coupling.
- Commonly used with Dependency Injection.

---

# Interview Tip

If asked:

**"Why are interfaces heavily used in .NET?"**

Mention these points:

- Dependency Injection
- Loose Coupling
- Unit Testing
- Extensibility
- Multiple Interface Implementation