# Composition in C#

## 1. What Is Composition?

Composition is an object-oriented design technique in which a class builds its behavior by containing or using objects of other classes.

It is commonly described as a **has-a relationship**.

For example:
- A car has an engine.
- An order has order items.
- A reporting service uses a report formatter.

Composition encourages classes to delegate responsibilities to collaborators rather than placing all logic in one large class.

## 2. Basic Example

```csharp
public class Engine
{
    public void Start()
    {
        Console.WriteLine("Engine started.");
    }
}

public class Car
{
    private readonly Engine _engine;

    public Car(Engine engine)
    {
        _engine = engine
            ?? throw new ArgumentNullException(nameof(engine));
    }

    public void Start()
    {
        _engine.Start();
        Console.WriteLine("Car is ready.");
    }
}
```

Usage:

```csharp
var engine = new Engine();
var car = new Car(engine);

car.Start();
```

The `Car` delegates engine-starting behavior to the `Engine` object.

**Important:** Composition does not necessarily mean that the containing class creates or exclusively owns the dependency. Those details depend on the design and object lifecycle.

## 3. Why Is Composition Useful?

- **Separation of responsibilities:** Each class handles a focused task.
- **Reusability:** Different classes can use the same component.
- **Testability:** Dependencies can be replaced with test doubles.
- **Flexibility:** Behavior can change without redesigning an inheritance hierarchy.
- **Maintainability:** Changes are easier to isolate.

## 4. Composition vs Inheritance

Inheritance represents an is-a relationship, while composition commonly represents a has-a relationship.

```csharp
// Inheritance: an ElectricCar is a Car.
public class Car { }

public class ElectricCar : Car { }

// Composition: a Car has an Engine.
public class Engine { }

public class Vehicle
{
    private readonly Engine _engine;

    public Vehicle(Engine engine)
    {
        _engine = engine;
    }
}
```

Use inheritance when a genuine subtype relationship exists and the derived class can safely substitute for the base class.

Use composition when a class needs another object's functionality or when behavior should be replaceable.

## 5. Business Scenario: Report Generation

Suppose an application must generate reports in CSV, PDF, and Excel formats.

Instead of building a large class with many conditional branches, define a formatter contract.

```csharp
public interface IReportFormatter
{
    string Format(string data);
}

public class CsvFormatter : IReportFormatter
{
    public string Format(string data)
    {
        return $"CSV: {data}";
    }
}

public class ReportService
{
    private readonly IReportFormatter _formatter;

    public ReportService(IReportFormatter formatter)
    {
        _formatter = formatter;
    }

    public string Generate(string data)
    {
        return _formatter.Format(data);
    }
}
```

Usage:

```csharp
var service = new ReportService(new CsvFormatter());

Console.WriteLine(service.Generate("Monthly Sales"));
```

To support another format, implement `IReportFormatter` and configure the appropriate implementation. `ReportService` does not need to know the formatting details.

This is composition combined with interface-based programming.

## 6. Composition and Dependency Injection

Dependency injection is a way to provide dependencies to a class. Constructor injection makes required dependencies explicit.

In ASP.NET Core:

```csharp
builder.Services.AddScoped<IReportFormatter, CsvFormatter>();
builder.Services.AddScoped<ReportService>();
```

The dependency injection container constructs `ReportService` and supplies its formatter.

For a production system, consider how the formatter is selected when multiple formats are supported. A factory, keyed services, or a strategy-selection component may be appropriate, depending on the requirements and .NET version.

## 7. Interview Questions and Answers

### Q1. What is composition?

**Answer:** Composition is a design technique in which an object collaborates with other objects to provide behavior. It helps separate responsibilities and makes components easier to replace and test.

### Q2. Why is composition often preferred over inheritance?

**Answer:** Composition allows behavior to vary independently of the class hierarchy. It can reduce tight coupling between base and derived classes. However, inheritance remains useful for genuine subtype relationships.

### Q3. Is composition the same as dependency injection?

**Answer:** No. Composition is a design relationship or technique. Dependency injection is a way to provide an object's dependencies. DI is frequently used to implement composition.

### Q4. Does composition require the containing class to create the dependency?

**Answer:** No. The dependency may be created internally or provided externally. Constructor injection is common when a dependency is required.

### Q5. How does composition improve unit testing?

**Answer:** A class can receive an abstraction through its constructor, allowing a test to supply a fake or mock dependency and verify behavior in isolation.

### Q6. Can composition be overused?

**Answer:** Yes. Creating too many tiny abstractions or layers without a meaningful reason can make the code harder to follow. Components should have clear responsibilities and useful boundaries.

## 8. Senior-Level Scenario Questions

### Scenario 1: A class has 800 lines and handles validation, database access, email, and PDF generation. What would you do?

**Suggested answer:** Identify distinct responsibilities and extract focused collaborators. For example, separate validation, persistence, notification, and document generation. Keep business rules in the appropriate application or domain component rather than splitting code mechanically.

### Scenario 2: You need to change notification from email to SMS without changing order-processing logic. How would you design it?

**Suggested answer:** Define a notification abstraction, inject it into the order-processing component, and select the implementation through configuration or a strategy. If multiple channels must be used together, model that requirement explicitly.

### Scenario 3: How would you unit-test a service that uses an external API?

**Suggested answer:** Place the external API behind an appropriate abstraction, inject it, and use a fake or mock in unit tests. Test the real integration separately with integration tests.

### Scenario 4: When might composition be worse than inheritance?

**Suggested answer:** Composition may introduce unnecessary delegation and objects when a stable, genuine subtype relationship already exists. Use the simplest design that accurately represents the domain and supports expected changes.

### Scenario 5: How would you prevent a composed object from becoming tightly coupled to its dependency?

**Suggested answer:** Keep dependencies focused, use abstractions where they provide meaningful flexibility, avoid exposing unnecessary implementation details, and enforce clear ownership and lifecycle rules.

## 9. Common Mistakes

- Confusing composition with dependency injection.
- Using interfaces for every class without a design reason.
- Hiding too much business logic inside a generic helper.
- Creating circular dependencies between collaborators.
- Ignoring dependency lifetimes in ASP.NET Core.
- Assuming composition automatically guarantees loose coupling.

## 10. Quick Revision

- Composition commonly represents a has-a relationship.
- Objects collaborate instead of relying only on inheritance.
- Constructor injection is a common implementation technique.
- Composition improves flexibility, testing, and separation of responsibilities.
- Choose abstractions based on real variation or testing needs, not by default.