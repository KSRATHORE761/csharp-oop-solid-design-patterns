# Interface vs Abstract Class in C#

## 1. What is an Interface?

An interface defines a contract that a class or struct agrees to implement. It describes capabilities without requiring a particular inheritance hierarchy.

```csharp
public interface IPaymentProcessor
{
    Task ProcessPaymentAsync(decimal amount);
}

public class CardPaymentProcessor : IPaymentProcessor
{
    public Task ProcessPaymentAsync(decimal amount)
    {
        Console.WriteLine($"Processing card payment: {amount}");
        return Task.CompletedTask;
    }
}
```

## 2. What is an Abstract Class?

An abstract class is a base class that cannot be instantiated directly. It can define shared state, constructors, implemented methods, and abstract members that derived classes must implement.

```csharp
public abstract class PaymentProcessor
{
    protected string Currency { get; }

    protected PaymentProcessor(string currency)
    {
        Currency = currency;
    }

    public void LogPayment(decimal amount)
    {
        Console.WriteLine($"{amount} {Currency}");
    }

    public abstract Task ProcessPaymentAsync(decimal amount);
}

public class CardPaymentProcessor : PaymentProcessor
{
    public CardPaymentProcessor() : base("INR") { }

    public override Task ProcessPaymentAsync(decimal amount)
    {
        Console.WriteLine($"Processing card payment: {amount} {Currency}");
        return Task.CompletedTask;
    }
}
```

## 3. Key Differences

| Feature | Interface | Abstract class |
|---|---|---|
| Main purpose | Define a contract/capability | Share a common base and implementation |
| Multiple inheritance | A class can implement multiple interfaces | A class can inherit only one class |
| Instance fields | Cannot declare ordinary instance fields | Can contain instance fields |
| Constructors | No instance constructors | Can have constructors |
| Implemented methods | Can have default implementations in modern C# | Can have concrete methods |
| Access modifiers | Interface members are generally public contracts; modern C# supports additional member forms | Supports protected, private, public, and other applicable access levels |
| State | No ordinary per-instance fields | Can maintain instance state |
| Best fit | Independent capabilities and loose coupling | Closely related types with shared state or behavior |

**Important:** Modern C# interfaces can include default implementations, static abstract members, and other advanced members. Therefore, “interfaces can never contain implementation” is not universally correct.

## 4. When Should You Use Each?

### Use an interface when:
- Unrelated classes need to provide the same capability.
- You want loose coupling and dependency injection.
- Multiple implementations are expected.
- You want to mock a dependency in unit tests.

### Use an abstract class when:
- Derived classes share meaningful state or implementation.
- There is a genuine common base type.
- You need protected members or constructor-enforced initialization.
- The base class can define a stable common workflow.

You can use both together when appropriate.

## 5. Real-World Scenario: Payment Processing

The business supports card, UPI, and net banking payments.

```csharp
public interface IPaymentProcessor
{
    Task ProcessAsync(decimal amount);
}

public class UpiPaymentProcessor : IPaymentProcessor
{
    public Task ProcessAsync(decimal amount)
    {
        Console.WriteLine($"Processing UPI: {amount}");
        return Task.CompletedTask;
    }
}

public class CheckoutService
{
    private readonly IPaymentProcessor _processor;

    public CheckoutService(IPaymentProcessor processor)
    {
        _processor = processor;
    }

    public Task CheckoutAsync(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        return _processor.ProcessAsync(amount);
    }
}
```

The checkout service depends on the contract, not a specific payment implementation. This makes it easier to add or replace payment processors and to test the checkout logic independently.

In ASP.NET Core, register the implementation using dependency injection:

```csharp
services.AddScoped<IPaymentProcessor, UpiPaymentProcessor>();
```

This registration selects one implementation for that service contract. If the application needs multiple payment methods, use an appropriate selection strategy rather than assuming one registration automatically selects the correct processor for every payment.

## 6. Interview Questions and Answers

### Q1. Can a class implement multiple interfaces?

**Answer:** Yes. A class can implement multiple interfaces, which allows it to support several independent capabilities without multiple class inheritance.

### Q2. Can an abstract class implement an interface?

**Answer:** Yes. An abstract class can implement an interface and leave some members unimplemented for its derived classes, or provide implementations itself.

### Q3. Can an interface have a constructor?

**Answer:** No instance constructor. An interface does not represent an object instance with constructor-managed instance state.

### Q4. Can an abstract class have a constructor?

**Answer:** Yes. Its constructor runs as part of constructing a derived-class instance and can initialize shared state.

### Q5. Can an abstract class contain non-abstract methods?

**Answer:** Yes. It can contain fully implemented methods alongside abstract members.

### Q6. Can an interface contain implemented methods?

**Answer:** Yes. Modern C# supports default interface implementations. However, interfaces are still most commonly used to express contracts and capabilities.

### Q7. Can we create an object of an abstract class?

**Answer:** No. You cannot instantiate it directly, but you can use an abstract-class reference to refer to an instance of a concrete derived class.

### Q8. Why prefer interfaces for service dependencies?

**Answer:** They reduce direct coupling to concrete implementations and make substitution, testing, and extension easier. An interface alone does not guarantee good design; its responsibilities must still be well defined.

## 7. Senior-Level Scenario Questions

### Scenario 1: A service needs both logging and notification capabilities. What would you use?

**Suggested answer:** Separate interfaces such as `ILogger` and `INotificationSender` are suitable if these are independent capabilities. Avoid forcing unrelated responsibilities into a single large interface.

### Scenario 2: Three payment processors share currency validation and logging. Should you use an abstract class?

**Suggested answer:** Possibly. An abstract base class can centralize genuinely common implementation. However, if processors do not share a stable base relationship, composition with reusable services may be more flexible.

### Scenario 3: A new implementation must be introduced without changing the checkout service. How would you design it?

**Suggested answer:** Make the checkout service depend on an abstraction, inject the implementation, and select the appropriate implementation through configuration, a factory, or another explicit strategy.

### Scenario 4: When can an abstract base class become a problem?

**Suggested answer:** When it accumulates unrelated responsibilities, makes derived classes depend on behavior they do not need, or becomes difficult to change because every derived type depends on it.

### Scenario 5: Is an interface always better than an abstract class?

**Suggested answer:** No. Interfaces are useful for contracts and independent capabilities. Abstract classes are useful for shared state and implementation in a genuine hierarchy. The design should reflect the domain and expected change.

## 8. Common Mistakes

- Saying interfaces cannot contain any implementation.
- Creating an abstract class merely to avoid writing an interface.
- Using inheritance only to reuse a few lines of code.
- Putting unrelated responsibilities into one interface.
- Assuming dependency injection automatically chooses the correct implementation.
- Making a base class responsible for too many unrelated concerns.

## 9. Quick Revision

- **Interface:** Defines a contract or capability.
- **Abstract class:** Provides a common base that may contain shared state and implementation.
- A class can implement multiple interfaces but inherit from only one class.
- Use interfaces for flexible contracts and abstract classes for meaningful shared behavior.
- Senior-level decisions should consider coupling, testing, extensibility, and business requirements.