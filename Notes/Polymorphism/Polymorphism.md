# 07 - Polymorphism (Part 1)
## Compile-Time Polymorphism (Method & Constructor Overloading)

---

# Prerequisites

Before learning Polymorphism, you should be comfortable with:

- Encapsulation
- Inheritance

---

# What is Polymorphism?

**Polymorphism** means **"One Interface, Many Forms."**

It allows the same method or interface to behave differently depending on how it is used.

> **Interview Definition**
>
> Polymorphism is one of the four pillars of Object-Oriented Programming (OOP). It allows objects to perform the same operation in different ways.

---

# Why Do We Need Polymorphism?

Imagine a Calculator.

Without Polymorphism:

```csharp
AddTwoNumbers();
AddThreeNumbers();
AddDecimalNumbers();
```

Every new variation requires a new method.

With Polymorphism:

```csharp
Add();
```

The compiler automatically chooses the correct method based on the parameters.

This results in:

- Cleaner code
- Better readability
- Easier maintenance
- Reusable methods

---

# Types of Polymorphism

Polymorphism is divided into two types.

| Type | Achieved Using |
|------|----------------|
| Compile-Time Polymorphism | Method Overloading, Constructor Overloading |
| Runtime Polymorphism | Method Overriding (`virtual`, `override`) |

---

# Compile-Time Polymorphism

Compile-time polymorphism is also known as:

- Static Polymorphism
- Early Binding

The compiler decides **which method to call during compilation.**

---

# Method Overloading

## Definition

Method Overloading means creating multiple methods having:

- Same method name
- Different parameter list

The compiler automatically selects the correct method.

---

## Example

```csharp
public class Calculator
{
    public int Add(int a, int b)
    {
        return a + b;
    }

    public int Add(int a, int b, int c)
    {
        return a + b + c;
    }

    public double Add(double a, double b)
    {
        return a + b;
    }
}
```

Usage

```csharp
Calculator calculator = new Calculator();

calculator.Add(10, 20);

calculator.Add(10, 20, 30);

calculator.Add(10.5, 15.2);
```

The compiler decides which `Add()` method should execute.

---

# Why Method Overloading?

Without overloading:

```csharp
AddIntegers();
AddDecimals();
AddThreeIntegers();
```

With overloading:

```csharp
Add();
```

The API becomes cleaner and easier to understand.

---

# Constructor Overloading

Constructors can also be overloaded.

Example:

```csharp
public class Employee
{
    public Employee()
    {

    }

    public Employee(string name)
    {

    }

    public Employee(string name, int age)
    {

    }
}
```

This allows creating objects with different levels of information.

---

# Rules of Method Overloading

## ✅ Allowed

### Different Number of Parameters

```csharp
Print()

Print(string name)
```

---

### Different Parameter Types

```csharp
Print(int number)

Print(string text)
```

---

### Different Parameter Order

```csharp
Print(int age, string name)

Print(string name, int age)
```

---

## ❌ Not Allowed

### Changing Only Return Type

```csharp
int Add(int a, int b)

double Add(int a, int b)
```

Compilation Error

---

### Changing Only Parameter Names

```csharp
Add(int x, int y)

Add(int a, int b)
```

Compilation Error

---

### Changing Only Access Modifier

```csharp
public Add(int a)

private Add(int a)
```

Compilation Error

---

# Method Signature

A Method Signature consists of:

- Method Name
- Number of Parameters
- Parameter Types
- Parameter Order

It **does NOT include**:

- Return Type
- Access Modifier
- Parameter Names

Example

```csharp
Display(int age, string name)
```

Method Signature

```text
Display(int, string)
```

---

# How Does the Compiler Choose the Correct Method?

The compiler checks:

1. Method name
2. Number of parameters
3. Parameter types
4. Parameter order

If an exact match exists, that method is called.

---

# Advantages of Method Overloading

- Improves readability
- Increases code reusability
- Reduces duplicate method names
- Makes APIs cleaner
- Easier maintenance

---

# When Should You Use Method Overloading?

Use overloading when all methods perform the **same logical operation** but accept different input.

Good Example

```csharp
Add()

Log()

Print()

Calculate()
```

Bad Example

```csharp
SaveCustomer()

DeleteCustomer()
```

These perform different operations and should not be overloaded.

---

# Best Practices

- Overload methods only for similar behavior.
- Keep overloads easy to understand.
- Avoid creating too many overloads.
- Prefer optional parameters only when they improve readability.

---

# Common Mistakes

### ❌ Overloading unrelated methods

Bad

```csharp
Process(Customer customer)

Process(Order order)
```

If both methods perform completely different work, use different method names.

---

### ❌ Thinking Return Type is Part of Signature

Wrong

```csharp
int Add(int a, int b)

double Add(int a, int b)
```

---

### ❌ Creating Ambiguous Overloads

Bad

```csharp
Print(int number)

Print(long number)
```

Passing a small integer can sometimes create ambiguity.

---

# Interview Questions

### What is Polymorphism?

Polymorphism allows the same interface or method to behave differently for different inputs or objects.

---

### What are the two types of Polymorphism?

- Compile-Time Polymorphism
- Runtime Polymorphism

---

### How is Compile-Time Polymorphism achieved?

Using:

- Method Overloading
- Constructor Overloading

---

### Can Constructors be Overloaded?

Yes.

---

### Can Static Methods be Overloaded?

Yes.

---

### Can Private Methods be Overloaded?

Yes.

---

### Can Return Type Differentiate Overloaded Methods?

No.

Return type is not part of the method signature.

---

### What is Method Signature?

Method Signature includes:

- Method Name
- Number of Parameters
- Parameter Types
- Parameter Order

---

### Is Access Modifier Part of Method Signature?

No.

---

### Is Parameter Name Part of Method Signature?

No.

---

# Senior Developer Perspective

Use Method Overloading only when the methods represent the **same business operation**.

Example from .NET:

```csharp
logger.Log(string message);

logger.Log(Exception exception);

logger.Log(LogLevel level, string message);
```

All methods perform **logging**, so overloading is appropriate.

Avoid overloading methods that perform completely different business operations.

---

# Quick Revision

## Polymorphism

- One Interface, Many Forms

---

## Types

- Compile-Time
- Runtime

---

## Compile-Time Polymorphism

- Method Overloading
- Constructor Overloading

---

## Overloading Rules

✅ Different number of parameters

✅ Different parameter types

✅ Different parameter order

❌ Return type only

❌ Parameter names only

❌ Access modifier only

---

## Method Signature

Includes:

- Method Name
- Number of Parameters
- Parameter Types
- Parameter Order

Does NOT Include:

- Return Type
- Access Modifier
- Parameter Names

---
# 07 - Polymorphism (Part 2)
## Runtime Polymorphism

---

# Runtime Polymorphism

Runtime Polymorphism is also known as:

- Dynamic Polymorphism
- Late Binding

The method to execute is determined **at runtime**, not during compilation.

It is achieved using:

- `virtual`
- `override`

---

# Why Do We Need Runtime Polymorphism?

Suppose every vehicle starts differently.

Without Runtime Polymorphism:

```csharp
if(vehicle is Car)
{
    // Start Car
}
else if(vehicle is Bike)
{
    // Start Bike
}
else if(vehicle is ElectricCar)
{
    // Start Electric Car
}
```

Whenever a new vehicle type is added, this code must be modified.

With Runtime Polymorphism:

```csharp
vehicle.Start();
```

Each object decides which implementation to execute.

---

# Method Overriding

## Definition

Method Overriding allows a derived class to provide its own implementation of a method already defined in the base class.

---

# virtual Keyword

A method must be marked as **virtual** if you want derived classes to override it.

Example

```csharp
public class Vehicle
{
    public virtual void Start()
    {
        Console.WriteLine("Vehicle Started");
    }
}
```

---

# override Keyword

A derived class overrides the base implementation using the **override** keyword.

```csharp
public class Car : Vehicle
{
    public override void Start()
    {
        Console.WriteLine("Car Started");
    }
}
```

---

# Runtime Method Dispatch

```csharp
Vehicle vehicle = new Car();

vehicle.Start();
```

Output

```text
Car Started
```

Although the reference type is `Vehicle`, the actual object is `Car`.

The CLR executes the overridden method.

---

# virtual vs override

| virtual | override |
|----------|-----------|
| Declared in Base Class | Declared in Derived Class |
| Allows overriding | Replaces base implementation |
| Only once | Can appear in every derived class |

---

# base Keyword

The `base` keyword accesses members of the base class.

Example

```csharp
public override void Start()
{
    base.Start();

    Console.WriteLine("Checking Air Conditioning");
}
```

Output

```text
Vehicle Started
Checking Air Conditioning
```

---

# Why Use base?

Use `base` when you want to extend the existing behavior instead of completely replacing it.

---

# Upcasting

## Definition

Converting a derived object into a base class reference.

```csharp
Vehicle vehicle = new Car();
```

No explicit cast is required.

---

## Why Upcasting?

It allows writing generic code.

Instead of

```csharp
Car car = new Car();

Bike bike = new Bike();
```

Use

```csharp
List<Vehicle> vehicles = new();
```

The collection can store:

- Car
- Bike
- ElectricCar

---

# Downcasting

## Definition

Converting a base reference back into a derived type.

```csharp
Vehicle vehicle = new Car();

Car car = (Car)vehicle;
```

---

## Risk

If the object is not actually a `Car`, an exception occurs.

```csharp
Vehicle vehicle = new Bike();

Car car = (Car)vehicle;
```

Output

```text
InvalidCastException
```

---

# is Operator

Checks an object's type safely.

```csharp
if(vehicle is Car)
{
    Console.WriteLine("Vehicle is a Car");
}
```

Modern C#

```csharp
if(vehicle is Car car)
{
    car.Honk();
}
```

---

# as Operator

Attempts a safe cast.

```csharp
Car car = vehicle as Car;
```

If the cast fails:

```text
car == null
```

No exception is thrown.

---

# Difference Between Casting Techniques

| Technique | Exception | Safe |
|-----------|-----------|------|
| Direct Cast | Yes | No |
| is | No | Yes |
| as | No | Yes |

---

# Pattern Matching

Pattern Matching combines type checking and casting.

Example

```csharp
if(vehicle is Car car)
{
    car.Honk();
}
```

No additional cast is required.

---

# Pattern Matching with switch

```csharp
switch(vehicle)
{
    case Car car:
        car.Honk();
        break;

    case Bike bike:
        bike.Wheelie();
        break;

    case ElectricCar electricCar:
        electricCar.DriveEcoMode();
        break;
}
```

Cleaner than multiple `if-else` statements.

---

# Advantages of Runtime Polymorphism

- Loose coupling
- Extensible code
- Better maintainability
- Easier testing
- Supports Open/Closed Principle

---

# Best Practices

- Prefer overriding over large `if-else` chains.
- Mark methods `virtual` only when overriding is expected.
- Use `base` only when required.
- Prefer `is` or `as` over unsafe casts.
- Use Pattern Matching in modern C#.

---

# Common Mistakes

### ❌ Forgetting virtual

```csharp
public void Start()
```

Cannot be overridden.

---

### ❌ Forgetting override

```csharp
public void Start()
```

Creates a new method instead of overriding.

---

### ❌ Unsafe Downcasting

```csharp
Car car = (Car)vehicle;
```

Always verify the object type first.

---

### ❌ Overusing virtual

Not every method should be virtual.

Only make methods virtual when customization is expected.

---

# Interview Questions

### What is Runtime Polymorphism?

Runtime Polymorphism allows method calls to be resolved during program execution.

---

### Which keywords are required?

- virtual
- override

---

### Can a non-virtual method be overridden?

No.

---

### Difference between Overloading and Overriding?

| Overloading | Overriding |
|--------------|------------|
| Same Class | Base + Derived Class |
| Compile Time | Runtime |
| Different Parameters | Same Signature |

---

### What is Upcasting?

Converting a derived object into a base class reference.

---

### Is Upcasting Safe?

Yes.

---

### What is Downcasting?

Converting a base reference back into a derived object.

---

### Is Downcasting Safe?

Only if the object actually belongs to the derived type.

---

### Difference between is and as?

`is`

- Checks type
- Returns `bool`

`as`

- Performs safe casting
- Returns `null` if casting fails

---

### Why use Pattern Matching?

It combines type checking and casting into a single statement, making code cleaner and safer.

---

# Senior Developer Perspective

Runtime Polymorphism is used throughout ASP.NET Core.

Examples include:

- Dependency Injection

```csharp
ILogger logger = new FileLogger();
```

- Middleware Pipeline

```csharp
app.UseAuthentication();
```

- Entity Framework Core

```csharp
DbContext context = new AppDbContext();
```

- Logging Providers

```csharp
ILogger logger = new ConsoleLogger();
```

The application works with the **base abstraction**, while the runtime decides which implementation to execute.

---

# Quick Revision

## Runtime Polymorphism

- Dynamic Binding
- Late Binding

---

## Keywords

- virtual
- override
- base

---

## Casting

### Upcasting

```csharp
Vehicle vehicle = new Car();
```

Safe

---

### Downcasting

```csharp
Car car = (Car)vehicle;
```

Risky

---

## Safe Casting

```csharp
is

as
```

---

## Pattern Matching

```csharp
if(vehicle is Car car)
```

```csharp
switch(vehicle)
```

---

# Summary

In this chapter, you learned:

- Runtime Polymorphism
- Method Overriding
- `virtual`
- `override`
- `base`
- Runtime Method Dispatch
- Upcasting
- Downcasting
- `is`
- `as`
- Pattern Matching
- Best Practices
- Common Mistakes
- Interview Questions
- Senior Developer Perspective

---

## Project

Next, we'll enhance the **Vehicle Management System** to demonstrate Runtime Polymorphism by:

- Overriding methods in `Car`, `Bike`, and `ElectricCar`
- Storing different vehicle types in a `List<Vehicle>`
- Demonstrating Upcasting and Downcasting
- Using `is`, `as`, and Pattern Matching with the project