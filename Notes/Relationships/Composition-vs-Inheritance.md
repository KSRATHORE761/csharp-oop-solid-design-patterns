# Composition vs Inheritance in C#

## 1. Overview

Composition and inheritance are two different ways to reuse and organize behavior.

- **Inheritance:** A derived class extends or specializes a base class. It represents an is-a relationship.
- **Composition:** A class collaborates with other objects to provide functionality. It commonly represents a has-a relationship.

A senior developer should choose based on the business domain, expected changes, substitutability, and maintainability—not just code reuse.

## 2. Key Differences

| Feature | Composition | Inheritance |
|---|---|---|
| Relationship | Has-a / uses-a | Is-a |
| Behavior reuse | Delegates to collaborators | Inherits or overrides base behavior |
| Flexibility | Dependencies can often be replaced | Behavior is tied more closely to the class hierarchy |
| Multiple capabilities | Can combine several collaborators | A class has only one direct base class |
| Coupling | Often lower when dependencies are well designed | Derived classes depend on base-class behavior and contracts |
| Testing | Collaborators can be substituted | Derived classes can be tested, but base behavior may be coupled |
| Best fit | Independent, interchangeable responsibilities | Genuine subtype relationships |

Neither approach is always superior. Composition can become unnecessarily complicated, while inheritance can be appropriate for a stable hierarchy.

## 3. Inheritance Example

```csharp
public class Vehicle
{
    public virtual void Start()
    {
        Console.WriteLine("Vehicle started.");
    }
}

public class Car : Vehicle
{
    public override void Start()
    {
        Console.WriteLine("Car started.");
    }
}
```

A car is a vehicle, so inheritance can represent the domain relationship.

However, a derived class should remain compatible with the expectations of the base class. Overriding methods should not break the base class's contract.

## 4. Composition Example

Suppose vehicles can have different engine implementations.

```csharp
public interface IEngine
{
    void Start();
}

public class PetrolEngine : IEngine
{
    public void Start()
    {
        Console.WriteLine("Petrol engine started.");
    }
}

public class ElectricMotor : IEngine
{
    public void Start()
    {
        Console.WriteLine("Electric motor started.");
    }
}

public class Car
{
    private readonly IEngine _engine;

    public Car(IEngine engine)
    {
        _engine = engine
            ?? throw new ArgumentNullException(nameof(engine));
    }

    public void Start()
    {
        _engine.Start();
    }
}
```

Usage:

```csharp
var petrolCar = new Car(new PetrolEngine());
var electricCar = new Car(new ElectricMotor());

petrolCar.Start();
electricCar.Start();
```

The `Car` delegates starting behavior to its engine. The engine implementation can vary without creating a separate `Car` subclass for every engine type.

## 5. The Senior-Level Design Principle

**Favor composition over inheritance when you need to vary behavior independently of the type hierarchy.**

Consider a notification system supporting email, SMS, and push notifications.

An inheritance-heavy design might create subclasses for every combination of channel and notification type. This can lead to many classes and duplicated behavior.

A composition-based design can combine a notification service with one or more channel implementations.

However, do not eliminate valid inheritance relationships merely to follow a rule. A `Car` being a `Vehicle` is a reasonable subtype relationship if the `Vehicle` contract accurately describes all supported vehicles.

## 6. Interview Questions and Answers

### Q1. What is the difference between composition and inheritance?

**Answer:** Inheritance reuses and specializes behavior through a base-class relationship. Composition builds behavior by collaborating with other objects. Inheritance represents an is-a relationship; composition commonly represents a has-a or uses-a relationship.

### Q2. Why is composition often preferred over inheritance?

**Answer:** Composition can reduce coupling to a class hierarchy and allows collaborators to be changed independently. It is useful when behavior varies across dimensions. However, inheritance is appropriate when there is a genuine and stable subtype relationship.

### Q3. What is the diamond problem?

**Answer:** In languages that support multiple class inheritance, the diamond problem occurs when a class inherits the same base class through multiple paths, creating ambiguity around inherited members and state. C# avoids multiple class inheritance but allows implementing multiple interfaces.

### Q4. Can composition completely replace inheritance?

**Answer:** No. Composition is excellent for combining behavior, while inheritance can express meaningful subtyping and shared base-class contracts. Many well-designed applications use both.

### Q5. What is the danger of deep inheritance hierarchies?

**Answer:** Behavior becomes harder to trace, base-class changes can affect many derived types, and subclasses may inherit behavior they do not need. Deep hierarchies can also make testing and extension more difficult.

### Q6. How does composition help the Open/Closed Principle?

**Answer:** If a class depends on a stable abstraction, new implementations can often be introduced without changing the class's core logic. This supports extension, although the design must still account for selection, configuration, and new requirements.

### Q7. Does composition always mean loose coupling?

**Answer:** No. A class can be composed of concrete dependencies and remain tightly coupled. Abstractions, clear responsibilities, and appropriate dependency boundaries can improve flexibility.

## 7. Senior-Level Scenario Questions

### Scenario 1: You have `PetrolCar`, `DieselCar`, `ElectricCar`, `HybridCar`, and separate classes for automatic and manual transmission. How would you redesign this?

**Suggested answer:** Identify independent dimensions such as powertrain and transmission. Use a vehicle hierarchy only where the is-a relationship is meaningful, and compose the vehicle with suitable powertrain and transmission abstractions. This avoids creating a subclass for every possible combination.

### Scenario 2: A base-class change breaks five derived classes. What does this suggest?

**Suggested answer:** The base class may expose an unstable contract, contain too many responsibilities, or require derived classes to depend on behavior they do not need. Review substitutability and consider extracting focused abstractions or using composition.

### Scenario 3: When would you keep inheritance instead of refactoring to composition?

**Suggested answer:** When the subtype relationship is genuine, the base class has a stable contract, and the derived types can honor that contract. A reasonable vehicle hierarchy is one example.

### Scenario 4: How would you make an external payment provider replaceable?

**Suggested answer:** Put the provider behind a suitable payment abstraction, inject it into the payment application service, and isolate provider-specific request and response mapping. Use a factory or explicit strategy when multiple providers must be selected dynamically.

### Scenario 5: A derived class overrides `Withdraw()` but silently allows invalid withdrawals. What is wrong?

**Suggested answer:** The derived implementation violates the behavioral expectations of the base contract. Review the base abstraction and enforce its invariants. If the derived type cannot safely satisfy the base contract, inheritance may be inappropriate.

### Scenario 6: How would you test a class that uses composition?

**Suggested answer:** Inject its collaborators through abstractions and use fakes or mocks to test its behavior in isolation. Also test important integrations with the actual implementations.

### Scenario 7: A developer extracts every method into a separate class. Is this good composition?

**Suggested answer:** Not necessarily. Excessive decomposition adds indirection and makes behavior harder to follow. Extract classes around cohesive responsibilities, meaningful variation, and useful testing boundaries.

## 8. Common Mistakes

- Using inheritance only to reuse a few methods.
- Treating every has-a relationship as a reason to create an interface.
- Creating deep inheritance hierarchies.
- Ignoring the behavioral contract of a base class.
- Assuming composition automatically eliminates coupling.
- Applying “favor composition over inheritance” as an absolute rule.

## 9. Quick Revision

- Inheritance: is-a relationship and subtype specialization.
- Composition: has-a or uses-a relationship and collaboration.
- Prefer composition when behavior needs to vary independently.
- Use inheritance when the subtype relationship and base contract are sound.
- Senior-level design should optimize for business correctness, changeability, and clarity—not simply fewer lines of code.