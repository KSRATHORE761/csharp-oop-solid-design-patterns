# Abstraction

---

# 1. Definition

**Abstraction** is the process of hiding implementation details and exposing only the essential functionality to the user.

It focuses on **what an object does** rather than **how it does it**.

---

# 2. Why Do We Need Abstraction?

Without abstraction, users need to understand every implementation detail of an object.

Abstraction helps us:

- Hide implementation complexity.
- Expose only required operations.
- Force derived classes to implement required behavior.
- Improve maintainability.
- Reduce code duplication.
- Make code easier to extend.

---

# 3. Real-World Examples

## ATM Machine

You can:

- Withdraw money
- Deposit money
- Check balance

You don't know:

- Database queries
- Network communication
- Banking server logic

The implementation is hidden.

---

## Car

You can:

- Start the engine
- Accelerate
- Brake

You don't know:

- Fuel injection
- Engine timing
- Combustion process

---

## Mobile Phone

You can:

- Make a call
- Send messages
- Open apps

You don't know how Android or iOS internally performs these operations.

---

# 4. How is Abstraction Achieved in C#?

C# provides two ways:

1. Abstract Class
2. Interface

---

# 5. Abstract Class

An **abstract class** is a class that **cannot be instantiated**.

It acts as a base class for other classes.

Example:

```csharp
public abstract class Vehicle
{
}
```

The following code is invalid:

```csharp
Vehicle vehicle = new Vehicle();
```

Reason:

An abstract class is incomplete by design.

---

# 6. Characteristics of Abstract Class

- Cannot create objects.
- Can contain constructors.
- Can contain fields.
- Can contain properties.
- Can contain implemented methods.
- Can contain abstract methods.
- Supports inheritance.
- Derived classes must implement abstract members.

---

# 7. Abstract Method

An abstract method has:

- No implementation
- Only declaration

Example:

```csharp
public abstract void Start();
```

Derived classes must implement it.

Example:

```csharp
public override void Start()
{
    Console.WriteLine("Starting Car...");
}
```

---

# 8. Concrete Method

A concrete method already has implementation.

Example:

```csharp
public void Brake()
{
    Console.WriteLine("Brake Applied");
}
```

Derived classes inherit this implementation.

---

# 9. Abstract vs Concrete Method

| Abstract Method | Concrete Method |
|-----------------|-----------------|
| No implementation | Has implementation |
| Must be overridden | Can be inherited directly |
| Uses abstract keyword | Normal method |
| Only inside abstract class | Any class |

---

# 10. Interview Questions

## Q1. What is Abstraction?

**Answer**

Abstraction is the process of hiding implementation details and exposing only essential functionality. It focuses on **what an object does** instead of **how it does it**.

---

## Q2. Why do we need Abstraction?

**Answer**

We use abstraction to:

- Hide complexity
- Improve maintainability
- Provide a common contract
- Force derived classes to implement required behavior

---

## Q3. Can we create an object of an abstract class?

**Answer**

No.

An abstract class cannot be instantiated because it is designed to be inherited by other classes.

Incorrect:

```csharp
Vehicle vehicle = new Vehicle();
```

---

## Q4. Can an abstract class contain implemented methods?

**Answer**

Yes.

An abstract class can contain both:

- Abstract methods
- Concrete methods

---

## Q5. Can an abstract class contain fields and properties?

**Answer**

Yes.

Unlike interfaces, abstract classes can have:

- Fields
- Properties
- Constructors
- Implemented methods
- Abstract methods

---

# 11. Common Mistakes

❌ Thinking an abstract class can only contain abstract methods.

✔ An abstract class can contain both abstract and concrete methods.

---

❌ Thinking abstract classes cannot have constructors.

✔ They can have constructors that initialize common state.

---

❌ Creating objects of an abstract class.

✔ Only derived classes can be instantiated.

---

# 12. Quick Revision

## Definition

Hide implementation, expose essential functionality.

---

## Keywords

- abstract class
- abstract method
- override

---

## Important Points

- Cannot instantiate abstract class.
- Can contain constructors.
- Can contain fields.
- Can contain implemented methods.
- Can contain abstract methods.
- Supports inheritance.

---

## Interview Tip

If the interviewer asks:

"What is abstraction?"

Don't stop at the definition.

Also mention:

- Hides implementation.
- Exposes essential functionality.
- Achieved using Abstract Classes and Interfaces in C#.


# Abstraction - Part 2

---

# 13. Constructors in Abstract Class

An abstract class **can have constructors**.

Although we cannot create an object of an abstract class directly, its constructor is called when a derived class object is created.

## Example

```csharp
public abstract class Vehicle
{
    protected Vehicle(string brand)
    {
        Brand = brand;
    }

    public string Brand { get; }
}

public class Car : Vehicle
{
    public Car(string brand) : base(brand)
    {
    }
}
```

When:

```csharp
Car car = new Car("Toyota");
```

Execution order:

```
Vehicle Constructor
        ↓
Car Constructor
```

---

# 14. Abstract Properties

Properties can also be abstract.

Example

```csharp
public abstract class Vehicle
{
    public abstract int MaxSpeed { get; }
}
```

Implementation

```csharp
public class Car : Vehicle
{
    public override int MaxSpeed => 220;
}
```

---

# 15. Can an Abstract Class Have Static Members?

Yes.

Example

```csharp
public abstract class Vehicle
{
    public static int Count { get; set; }
}
```

Static members belong to the class itself, not to an object.

---

# 16. When Should You Use an Abstract Class?

Use an abstract class when:

- Classes have a common base.
- They share common implementation.
- They share common state.
- Some behavior should be mandatory.
- You want code reuse.

Example

```
Vehicle
    ↓
Car
Bike
ElectricCar
```

---

# 17. When NOT to Use an Abstract Class

Avoid abstract classes when:

- Classes are unrelated.
- Only behavior needs to be shared.
- Multiple inheritance of behavior is required.

In these cases, prefer an **interface**.

---

# 18. Abstraction vs Encapsulation

| Abstraction | Encapsulation |
|-------------|---------------|
| Hides implementation | Hides data |
| Focuses on what | Focuses on how data is protected |
| Achieved using Abstract Class & Interface | Achieved using access modifiers |
| Reduces complexity | Protects object state |

Example

### Abstraction

```
Car.Start()
```

You don't know how the engine starts.

---

### Encapsulation

```
private decimal balance;
```

The balance cannot be modified directly.

---

# 19. Abstract Class vs Normal Class

| Abstract Class | Normal Class |
|----------------|--------------|
| Cannot instantiate | Can instantiate |
| Can contain abstract methods | Cannot contain abstract methods |
| Designed for inheritance | Can be used directly |
| Represents a base concept | Represents a complete object |

---

# 20. Advantages

- Hides complexity.
- Promotes code reuse.
- Enforces a common contract.
- Improves maintainability.
- Makes code extensible.
- Reduces duplication.

---

# 21. Limitations

- Supports only single inheritance.
- Tight coupling between base and derived classes.
- Not suitable when classes are unrelated.

---

# 22. Interview Questions

## Q1. Can an abstract class have constructors?

**Answer**

Yes.

Constructors initialize common state and are executed when a derived class object is created.

---

## Q2. Can an abstract class have implemented methods?

**Answer**

Yes.

An abstract class can contain both abstract and concrete methods.

---

## Q3. Can an abstract class have static methods?

**Answer**

Yes.

Static methods belong to the class, so they are allowed.

---

## Q4. Can an abstract class have fields?

**Answer**

Yes.

Unlike interfaces, abstract classes can contain fields, properties, constructors, and implemented methods.

---

## Q5. Can an abstract class implement an interface?

**Answer**

Yes.

Example

```csharp
public interface IService
{
    void Execute();
}

public abstract class BaseService : IService
{
    public abstract void Execute();
}
```

---

## Q6. When should you choose an abstract class instead of an interface?

**Answer**

Choose an abstract class when:

- Classes share common state.
- Classes share implementation.
- There is an **IS-A** relationship.
- Code reuse is required.

---

## Q7. Can an abstract class have private methods?

**Answer**

Yes.

Private methods are often used internally by concrete methods within the abstract class.

---

## Q8. Can abstract methods be private?

**Answer**

No.

Abstract methods must be overridden by derived classes, so they cannot be private.

---

## Q9. Can we mark an abstract method as static?

**Answer**

No.

Static methods cannot be overridden.

---

## Q10. Why can't we instantiate an abstract class?

**Answer**

Because it may contain abstract members without implementation, making it an incomplete type.

---

# 23. Common Mistakes

❌ Making every base class abstract.

✔ Only use abstraction when there is a clear common base.

---

❌ Using an abstract class for unrelated classes.

✔ Use interfaces instead.

---

❌ Forgetting to override abstract methods.

✔ Every concrete derived class must implement all abstract members.

---

❌ Confusing abstraction with encapsulation.

✔ Abstraction hides implementation.

✔ Encapsulation protects data.

---

# 24. Best Practices

- Keep abstract classes focused.
- Put only common functionality in the base class.
- Avoid large abstract classes.
- Don't force unnecessary members on derived classes.
- Prefer interfaces when only a contract is needed.

---

# 25. Interview Summary

## Definition

Hide implementation and expose only essential functionality.

---

## Keywords

- abstract
- abstract class
- abstract method
- override

---

## Remember

- Cannot instantiate.
- Can have constructors.
- Can have fields.
- Can have properties.
- Can have concrete methods.
- Can have abstract methods.
- Supports inheritance.
- Used for common state and shared implementation.

---

# Quick Revision

✅ Definition

✅ Why Abstraction?

✅ Abstract Class

✅ Abstract Method

✅ Concrete Method

✅ Constructors

✅ Properties

✅ Advantages

✅ Limitations

✅ Abstraction vs Encapsulation

✅ Interview Questions

✅ Common Mistakes