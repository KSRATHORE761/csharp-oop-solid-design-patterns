# 06 - Inheritance in C#

> **Difficulty:** ⭐⭐☆☆☆
>
> **Estimated Reading Time:** 45–60 Minutes
>
> **Prerequisites:**
>
> * Classes & Objects
> * Constructors
> * Properties
> * Access Modifiers
> * Encapsulation

---

# Learning Objectives

After completing this chapter, you will be able to:

* Explain Inheritance confidently in interviews.
* Understand why Inheritance is used.
* Implement Inheritance in C#.
* Explain different types of Inheritance.
* Understand constructor chaining and the `base` keyword.
* Know the advantages and disadvantages of Inheritance.
* Apply Inheritance in real-world applications.

---

# 1. What is Inheritance?

## Interview Definition (30 Seconds)

**Inheritance** is an Object-Oriented Programming (OOP) concept that allows a class to inherit the properties and behaviors of another class. It promotes **code reusability**, **extensibility**, and represents an **IS-A relationship** between classes.

---

## Simple Definition

Inheritance allows us to create a new class from an existing class so that the new class can reuse existing members and also define its own additional functionality.

---

## Example

Instead of writing common properties multiple times:

```text
Car
- Brand
- Model
- Start()

Bike
- Brand
- Model
- Start()
```

Create a common base class:

```text
Vehicle
│
├── Brand
├── Model
└── Start()

      ▲
      │
 ┌────┴────┐
 │         │
Car      Bike
```

Now `Car` and `Bike` automatically inherit common members from `Vehicle`.

---

# 2. Why Do We Need Inheritance?

Without inheritance, code duplication increases.

Example:

```text
Employee
- Name
- Email
- Salary

Manager
- Name
- Email
- Salary

Developer
- Name
- Email
- Salary
```

Every class repeats the same members.

Using inheritance:

```text
Employee
│
├── Name
├── Email
└── Salary

      ▲
      │
 ┌────┴─────┐
 │          │
Manager  Developer
```

Common members exist only once.

---

## Benefits

* Code Reusability
* Reduced Duplication
* Easier Maintenance
* Better Extensibility
* Better Code Organization
* Represents IS-A Relationship

---

# 3. Key Terminologies

## Base Class (Parent Class)

The class whose members are inherited.

Example:

```csharp
class Vehicle
{
}
```

Vehicle is the **Base Class**.

---

## Derived Class (Child Class)

The class that inherits another class.

```csharp
class Car : Vehicle
{
}
```

Car is the **Derived Class**.

---

## IS-A Relationship

Inheritance models an **IS-A** relationship.

Examples:

```text
Car IS-A Vehicle

Dog IS-A Animal

SavingsAccount IS-A BankAccount
```

Incorrect examples:

```text
Engine IS-A Car ❌

Wheel IS-A Car ❌
```

These represent **HAS-A** relationships, not inheritance.

---

# 4. Syntax of Inheritance

C# uses the `:` operator.

```csharp
public class Vehicle
{
    public void Start()
    {
        Console.WriteLine("Vehicle Started");
    }
}

public class Car : Vehicle
{
}
```

Usage:

```csharp
Car car = new Car();

car.Start();
```

Even though `Start()` is not defined in `Car`, it is available because it is inherited from `Vehicle`.

---

# 5. Types of Inheritance

Interviewers frequently ask this question.

There are **five** types of inheritance.

---

## 5.1 Single Inheritance

One child inherits from one parent.

```text
Vehicle
   │
   ▼
 Car
```

Supported in C#.

---

## 5.2 Multilevel Inheritance

A child becomes the parent of another class.

```text
Vehicle
   │
   ▼
 Car
   │
   ▼
 SportsCar
```

Supported in C#.

---

## 5.3 Hierarchical Inheritance

Multiple classes inherit from the same parent.

```text
        Vehicle
       /   |   \
      /    |    \
    Car   Bike  Truck
```

Supported in C#.

---

## 5.4 Multiple Inheritance

One class inherits from multiple base classes.

```text
A      B
 \    /
  \  /
   C
```

**Not supported for classes in C#.**

It is achieved using **Interfaces**.

---

## 5.5 Hybrid Inheritance

Combination of multiple inheritance types.

Not supported directly for classes because C# does not support multiple class inheritance.

---

# 6. Why Doesn't C# Support Multiple Class Inheritance?

The primary reason is the **Diamond Problem**.

Example:

```text
        Animal
        /    \
       /      \
    Dog      Cat
       \      /
        \    /
       Hybrid
```

Suppose both `Dog` and `Cat` contain:

```csharp
MakeSound();
```

Now `Hybrid` inherits from both.

Question:

> Which `MakeSound()` should be called?

This creates ambiguity.

To avoid this complexity, C# allows only **single class inheritance**.

If multiple behavior is required, C# uses **Interfaces**.

---

# 7. Constructor Chaining (Overview)

When an object of the derived class is created:

1. Base class constructor executes first.
2. Derived class constructor executes second.

Example:

```csharp
class Vehicle
{
    public Vehicle()
    {
        Console.WriteLine("Vehicle Constructor");
    }
}

class Car : Vehicle
{
    public Car()
    {
        Console.WriteLine("Car Constructor");
    }
}
```

Output:

```text
Vehicle Constructor
Car Constructor
```

We'll study this in more detail later.

---

# 8. The `base` Keyword (Overview)

The `base` keyword refers to the immediate parent class.

Common uses:

* Call the base constructor.
* Access base class methods.
* Access hidden members.

Example:

```csharp
public class Vehicle
{
    public Vehicle(string brand)
    {
    }
}

public class Car : Vehicle
{
    public Car() : base("Toyota")
    {
    }
}
```

---

# 9. Advantages of Inheritance

* Promotes code reuse.
* Reduces duplication.
* Improves maintainability.
* Makes applications easier to extend.
* Encourages cleaner architecture.
* Supports polymorphism (covered in the next chapter).

---

# 10. Limitations of Inheritance

Inheritance is powerful but should not be overused.

Common drawbacks:

* Tight coupling between parent and child classes.
* Changes in the base class may unintentionally affect derived classes.
* Deep inheritance hierarchies become difficult to understand.
* Incorrect inheritance leads to poor design.

> **Rule:** Inheritance should model an **IS-A** relationship. If that relationship doesn't exist, consider **Composition** instead.

---

# 11. Best Practices

✔ Keep inheritance hierarchies shallow.

✔ Reuse only common behavior.

✔ Use meaningful base classes.

✔ Don't inherit just to reuse a few lines of code.

✔ Favor Composition when an IS-A relationship doesn't exist.

✔ Design base classes carefully because changes affect all derived classes.

---

# 12. Common Mistakes

### ❌ Mistake 1

Using inheritance for code reuse only.

```text
Car inherits Logger
```

Incorrect.

---

### ❌ Mistake 2

Deep inheritance hierarchy.

```text
A
│
B
│
C
│
D
│
E
│
F
```

Difficult to maintain.

---

### ❌ Mistake 3

Ignoring the IS-A relationship.

Example:

```text
Engine inherits Car
```

Incorrect.

A Car **has an** Engine.

---

### ❌ Mistake 4

Creating large base classes.

A base class should only contain members common to every derived class.

---

# 13. Real-World .NET Examples

Inheritance is used extensively in .NET.

### Exception Hierarchy

```text
Exception
│
├── ArgumentException
├── InvalidOperationException
├── IOException
└── NullReferenceException
```

---

### Stream Hierarchy

```text
Stream
│
├── FileStream
├── MemoryStream
└── NetworkStream
```

---

### ASP.NET Core

```text
ControllerBase
      │
      ▼
ProductsController
```

Every controller inherits common functionality.

---

# Quick Revision

### Remember these points

* Inheritance models an **IS-A** relationship.
* C# supports **Single**, **Multilevel**, and **Hierarchical** inheritance.
* C# **does not** support multiple class inheritance.
* Multiple inheritance is achieved using **Interfaces**.
* Base constructor executes before derived constructor.
* `base` refers to the parent class.
* Use inheritance only when there is a genuine IS-A relationship.
* Prefer Composition over Inheritance when appropriate.

---

# Key Takeaways

* Inheritance allows one class to reuse the members of another class.
* It reduces duplication and improves maintainability.
* It should represent an **IS-A** relationship.
* Don't misuse inheritance simply for code reuse.
* Understanding inheritance is essential before learning **Polymorphism**, as runtime polymorphism depends on inheritance.

---

# Interview Corner


This section contains the most frequently asked inheritance interview questions for **4–8 years experienced .NET developers**. Learn the concepts, don't memorize the answers.

---

# 1. What is Inheritance?

### Answer

Inheritance is an OOP concept that allows one class to acquire the properties and behaviors of another class. It promotes code reuse, extensibility, and represents an **IS-A** relationship.

Example:

```csharp
public class Vehicle
{
    public void Start() { }
}

public class Car : Vehicle
{
}
```

`Car` automatically inherits `Start()`.

---

### Follow-up Questions

* Why do we need inheritance?
* How is it different from composition?
* Can inheritance increase coupling?

---

# 2. Why do we need Inheritance?

### Answer

Inheritance helps us:

* Reuse common code.
* Reduce duplication.
* Improve maintainability.
* Extend existing functionality.
* Build polymorphic applications.

Instead of rewriting common properties and methods in multiple classes, we place them in a base class.

---

# 3. Explain the IS-A Relationship.

### Answer

Inheritance should always represent an **IS-A** relationship.

Examples:

```
Car IS-A Vehicle

Dog IS-A Animal

SavingsAccount IS-A BankAccount
```

Incorrect examples:

```
Engine IS-A Car ❌

Wheel IS-A Car ❌
```

These are **HAS-A** relationships.

---

# 4. What are the different types of Inheritance?

### Answer

Five types:

* Single
* Multilevel
* Hierarchical
* Multiple
* Hybrid

In C# classes:

✔ Single

✔ Multilevel

✔ Hierarchical

❌ Multiple

❌ Hybrid

Multiple behavior is achieved through Interfaces.

---

# 5. Why doesn't C# support Multiple Class Inheritance?

### Answer

To avoid the **Diamond Problem**.

If two parent classes implement the same method, the compiler cannot determine which implementation the child class should inherit.

Instead, C# supports:

* Single class inheritance
* Multiple interface inheritance

---

### Interview Tip

Mentioning **Diamond Problem** immediately creates a good impression.

---

# 6. What is the Diamond Problem?

### Answer

```
        Animal
       /      \
     Dog      Cat
       \      /
      Hybrid
```

If both Dog and Cat implement

```
MakeSound()
```

Which implementation should Hybrid use?

This ambiguity is called the Diamond Problem.

---

# 7. Can a Constructor be inherited?

### Answer

No.

Constructors are **never inherited**.

However, when a derived object is created, the base class constructor executes automatically.

---

### Example

```csharp
Vehicle v = new Car();
```

Execution order

```
Vehicle Constructor

↓

Car Constructor
```

---

# 8. What is Constructor Chaining?

### Answer

Constructor chaining is the process where constructors are executed from the base class to the derived class.

Execution order:

```
GrandParent

↓

Parent

↓

Child
```

---

# 9. Why is the Base Constructor executed first?

### Answer

Because the child object depends on the base class being initialized first.

The parent class establishes the common state before the child adds its own members.

---

# 10. What is the base Keyword?

### Answer

The `base` keyword refers to the immediate parent class.

It is used to:

* Call the base constructor.
* Call base methods.
* Access hidden members.

Example:

```csharp
public Car(string brand)
    : base(brand)
{
}
```

---

# 11. Can Private Members be inherited?

### Answer

Yes.

Private members are inherited as part of the object, but they are **not directly accessible** from the derived class.

Example

```csharp
class Person
{
    private string name;
}
```

`Employee` inherits `name`, but cannot access it directly.

---

# 12. Which Access Modifier should we use for members accessed by child classes?

### Answer

Use **protected**.

```
private
```

Accessible only inside the same class.

```
protected
```

Accessible inside:

* Base class
* Derived classes

---

# 13. Can Static Members be inherited?

### Answer

Yes.

Static members belong to the type, not the object.

They are inherited but should generally be accessed using the class name.

---

# 14. Can a Sealed Class be inherited?

### Answer

No.

Example

```csharp
public sealed class Logger
{
}
```

This class cannot be inherited.

---

# 15. Can we inherit a Struct?

### Answer

No.

Structures cannot participate in class inheritance.

Every struct implicitly inherits from

```
System.ValueType
```

---

# 16. Difference between Inheritance and Composition

| Inheritance               | Composition           |
| ------------------------- | --------------------- |
| IS-A                      | HAS-A                 |
| Tight Coupling            | Loose Coupling        |
| Reuse through inheritance | Reuse through objects |
| Less flexible             | More flexible         |

Example

```
Car IS-A Vehicle
```

Inheritance.

```
Car HAS-A Engine
```

Composition.

---

# 17. When should we avoid Inheritance?

### Answer

Avoid inheritance when:

* There is no IS-A relationship.
* Only a small amount of code is reused.
* Base class changes frequently.
* Composition provides a simpler design.

---

# 18. What are the disadvantages of Inheritance?

### Answer

* Tight coupling.
* Fragile base class problem.
* Difficult to maintain deep hierarchies.
* Changes in parent affect all child classes.

---

# 19. Where have you used Inheritance in your projects?

### Sample Answer

> We use inheritance in ASP.NET Core where all controllers inherit from `ControllerBase`, which provides common functionality such as HTTP responses, model binding, and request handling. We also use inheritance in custom exception hierarchies and shared domain models where multiple entities share common properties.

---

# 20. Does Inheritance support Polymorphism?

### Answer

Yes.

Runtime polymorphism depends on inheritance.

Without inheritance, method overriding is not possible.

We'll study this in detail in the Polymorphism chapter.

---

# Common Mistakes During Interviews

### ❌ Saying inheritance is only for code reuse.

Better answer:

Inheritance is primarily used to model an **IS-A** relationship. Code reuse is a benefit, not the only purpose.

---

### ❌ Using inheritance everywhere.

Interviewers like candidates who know when **not** to use inheritance.

---

### ❌ Confusing IS-A and HAS-A.

Remember

```
Car IS-A Vehicle

Car HAS-A Engine
```

---

### ❌ Forgetting constructor execution order.

Always remember

```
Base Constructor

↓

Derived Constructor
```

---

# Interview Tips

When answering inheritance questions:

✔ Explain the concept.

✔ Explain why it exists.

✔ Give a practical example.

✔ Mention best practices.

✔ Mention limitations.

This makes your answer much stronger than simply giving a definition.

---

# One-Page Quick Revision

## Definition

Inheritance allows one class to inherit the properties and behaviors of another class.

---

## Purpose

* Code Reuse
* Extensibility
* Maintainability
* IS-A Relationship

---

## Types

✔ Single

✔ Multilevel

✔ Hierarchical

❌ Multiple

❌ Hybrid

---

## Keywords

```
:
base
protected
sealed
```

---

## Constructor Order

```
Base

↓

Derived
```

---

## Multiple Inheritance

Classes → ❌

Interfaces → ✔

---

## IS-A vs HAS-A

```
Car IS-A Vehicle

Car HAS-A Engine
```

---

## Best Practices

* Keep hierarchy shallow.
* Use meaningful base classes.
* Don't inherit only for code reuse.
* Prefer Composition when appropriate.

---

# Chapter Summary

After completing this chapter, you should be able to:

* Explain inheritance confidently.
* Implement inheritance in C#.
* Identify valid IS-A relationships.
* Explain constructor chaining.
* Use the `base` keyword correctly.
* Explain why C# doesn't support multiple class inheritance.
* Decide between inheritance and composition.
* Answer common inheritance interview questions with confidence.

---

