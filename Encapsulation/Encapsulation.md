# Encapsulation in C#

> "Encapsulation is not about hiding data. It is about protecting an object's state by controlling how it is accessed and modified."

---

# Table of Contents

1. What is Encapsulation?
2. Why do we need Encapsulation?
3. Problem without Encapsulation
4. Core Principles
5. Data Hiding vs Encapsulation
6. How Encapsulation is achieved in C#
7. Real-world Analogy
8. First C# Example
9. Key Takeaways

---

# 1. What is Encapsulation?

## Interview Definition

**Encapsulation is the process of bundling data (fields) and the methods that operate on that data into a single unit (class) while restricting direct access to the object's internal state.**

Instead of allowing external code to modify data directly, the object exposes controlled operations (methods or properties) that enforce business rules and maintain a valid state.

---

## Simple Definition

Encapsulation means:

- Hide implementation details.
- Protect object state.
- Allow only valid operations on an object.

Think of it as putting a protective boundary around an object.

---

## Keywords

- Data Hiding
- Controlled Access
- Object State
- Business Rules
- Validation
- Behavior
- Information Hiding

---

# 2. Why do we need Encapsulation?

Imagine there is no encapsulation.

```csharp
public class BankAccount
{
    public decimal Balance;
}
```

Now any code can do this:

```csharp
account.Balance = -5000;
```

or

```csharp
account.Balance = 1000000000;
```

Nothing stops the object from entering an invalid state.

Problems:

- No validation
- No business rules
- Invalid data
- Difficult debugging
- Difficult maintenance

As applications grow, this becomes one of the biggest causes of bugs.

Encapsulation solves this problem.

---

# 3. Problem without Encapsulation

Consider an Employee class.

```csharp
public class Employee
{
    public string Name;
    public decimal Salary;
}
```

Now another developer writes

```csharp
employee.Name = "";
employee.Salary = -10000;
```

The object now contains invalid data.

The Employee object has no control over its own state.

A well-designed class should never allow itself to become invalid.

This is the primary purpose of encapsulation.

---

# 4. Core Principles of Encapsulation

Encapsulation is based on three important principles.

## Principle 1 - Hide Internal Implementation

Users of the class should not know how the class performs its work.

Example

Instead of

```csharp
order.Status = "Delivered";
```

Prefer

```csharp
order.MarkAsDelivered();
```

The caller doesn't care how delivery is implemented.

The class decides.

---

## Principle 2 - Protect Object State

Objects should never allow invalid data.

For example

A student's age should never be negative.

A bank balance should not become invalid.

An order should not become "Delivered" before payment.

The object itself should enforce these rules.

---

## Principle 3 - Expose Behavior, Not Data

One of the most common senior interview questions.

Bad Design

```csharp
employee.Salary = 50000;
```

Better Design

```csharp
employee.IncreaseSalary(5000);
```

Why?

Because the method can

- validate input
- calculate tax
- log changes
- publish events
- enforce company policy

The object controls its behavior.

---

# 5. Data Hiding vs Encapsulation

Many developers think these terms are identical.

They are not.

## Data Hiding

Data hiding simply means preventing direct access to internal data.

Example

```csharp
private decimal balance;
```

The field cannot be accessed outside the class.

Its only goal is hiding data.

---

## Encapsulation

Encapsulation is a design principle.

It controls

- how data is accessed
- how data is modified
- when data is modified
- whether data is valid

Encapsulation uses data hiding as one of its techniques.

---

## Difference

| Data Hiding | Encapsulation |
|-------------|---------------|
| Technique | Design Principle |
| Hides data | Protects object state |
| Uses access modifiers | Uses fields, properties, methods, validation |
| Focuses on accessibility | Focuses on behavior and consistency |

---

## Interview Tip

> Data Hiding is a way to achieve Encapsulation, but Encapsulation is much broader than simply making fields private.

---

# 6. How is Encapsulation achieved in C#?

C# provides several language features.

## 1. Access Modifiers

```
private
public
protected
internal
protected internal
private protected
```

Among these, **private** is the most commonly used for encapsulation because it prevents direct access to implementation details.

---

## 2. Private Fields

```csharp
private decimal balance;
```

Private fields ensure no external code can modify the object's internal state directly.

---

## 3. Properties

Properties provide controlled access.

```csharp
public decimal Balance
{
    get { return balance; }
    private set
    {
        if(value < 0)
            throw new ArgumentException();

        balance = value;
    }
}
```

The property allows validation before updating the field.

---

## 4. Methods

Methods are preferred whenever business rules are involved.

Instead of

```csharp
student.Course = "M.Tech";
```

Prefer

```csharp
student.ChangeCourse("M.Tech");
```

Inside the method we can

- validate
- log
- notify
- update history
- enforce permissions

---

## 5. Constructor Validation

A constructor should create only valid objects.

Bad

```csharp
Student student = new Student("",-10,"");
```

Good

The constructor validates every value.

If validation fails,

Throw an exception.

This guarantees that every Student object is valid immediately after creation.

---

# 7. Real-world Analogy

## ATM Machine

You cannot open an ATM and directly edit its cash.

Instead, the ATM exposes only these operations.

```
Deposit()

Withdraw()

CheckBalance()
```

Everything else remains hidden.

Users interact with behavior, not internal implementation.

This is encapsulation.

---

# 8. First C# Example

```csharp
public class BankAccount
{
    private decimal balance;

    public decimal Balance
    {
        get => balance;
        private set
        {
            if (value < 0)
                throw new ArgumentException("Balance cannot be negative.");

            balance = value;
        }
    }

    public BankAccount(decimal openingBalance)
    {
        Balance = openingBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        if (amount > Balance)
            throw new InvalidOperationException("Insufficient balance.");

        Balance -= amount;
    }
}
```

### Why is this a good design?

- Private field
- Validation
- No public setter
- Behavior-driven methods
- Object always remains valid

---

# 9. Key Takeaways

- Encapsulation protects an object's state.
- It is **more than making fields private**.
- Data Hiding is one technique used to achieve Encapsulation.
- Prefer exposing **behavior** instead of allowing direct data modification.
- Constructors should create valid objects.
- Methods are preferred over setters when business rules exist.
- Well-encapsulated objects are easier to maintain, test, and extend.

---

## Quick Revision

✅ Hide implementation details

✅ Protect object state

✅ Expose behavior

✅ Validate every state change

✅ Constructor should create a valid object

✅ Prefer methods over setters

✅ Private fields + public behavior = Good encapsulation


---

# 10. Encapsulation Using Properties

Properties are one of the most common ways to implement encapsulation in C#.

A property acts as a controlled gateway to access or modify a private field.

Instead of exposing fields directly, we expose properties that can validate or restrict access.

---

## Why not expose fields directly?

❌ Bad Design

```csharp
public class Student
{
    public int Age;
}
```

Now anyone can write

```csharp
student.Age = -10;
```

The object is now in an invalid state.

---

## Better Design

```csharp
public class Student
{
    private int age;

    public int Age
    {
        get => age;
        private set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(Age));

            age = value;
        }
    }

    public Student(int age)
    {
        Age = age;
    }
}
```

Now the object controls how Age is updated.

---

# 11. Backing Field vs Auto-Property

One of the most common interview questions.

## Auto Property

```csharp
public string Name { get; set; }
```

Compiler automatically creates a hidden private field.

Equivalent to

```csharp
private string name;

public string Name
{
    get => name;
    set => name = value;
}
```

---

## When to use Auto Property?

Use it when

- No validation
- No custom logic
- Simple data storage

Example

```csharp
public DateTime CreatedOn { get; init; }

public Guid Id { get; init; }
```

---

## Backing Field

Use a backing field when you need

- Validation
- Logging
- Calculations
- Notifications
- Lazy loading

Example

```csharp
private decimal salary;

public decimal Salary
{
    get => salary;

    private set
    {
        if (value < 0)
            throw new ArgumentException();

        salary = value;
    }
}
```

---

## Interview Tip

Use Auto Properties for simple data.

Use Backing Fields when business logic is required.

---

# 12. Property Accessors

Properties contain two accessors.

## Getter

Returns a value.

```csharp
get => age;
```

---

## Setter

Updates a value.

```csharp
set
{
    age = value;
}
```

---

Accessors can have different access levels.

Example

```csharp
public string Name
{
    get;
    private set;
}
```

Meaning

Anyone can read.

Only this class can modify.

This is one of the most common patterns in enterprise applications.

---

# 13. Read-only Properties

Sometimes data should never change.

Example

Employee Id

Student Id

Order Number

Invoice Number

Example

```csharp
public int Id { get; }

public Student(int id)
{
    Id = id;
}
```

After object creation

```
Id cannot change.
```

This improves data integrity.

---

# 14. Init-only Properties (C# 9+)

Introduced to support immutable objects.

Example

```csharp
public class Student
{
    public int Id { get; init; }

    public string Name { get; init; }
}
```

Usage

```csharp
Student student = new()
{
    Id = 1,
    Name = "Kuldeep"
};
```

After object creation

```
student.Name = "ABC";
```

❌ Not allowed

---

Interview Question

Difference between

```
private set
```

and

```
init
```

Answer

private set

- Property can change inside the class at any time.

init

- Property can only be assigned during object creation.

---

# 15. Object Invariants

One of the most important concepts.

Very few beginners know this.

## What is an Invariant?

An invariant is a rule that must always remain true throughout an object's lifetime.

Examples

Bank Account

```
Balance >= 0
```

Student

```
Age >= 0
```

Employee

```
Salary >= MinimumSalary
```

Order

```
Delivered only after Payment
```

Encapsulation helps protect these invariants.

---

## Example

```csharp
public class BankAccount
{
    public decimal Balance { get; private set; }

    public void Withdraw(decimal amount)
    {
        if (amount > Balance)
            throw new InvalidOperationException();

        Balance -= amount;
    }
}
```

Invariant

```
Balance can never become negative.
```

---

# 16. Why Methods are Better than Public Setters

Many developers write

```csharp
public decimal Salary
{
    get;
    set;
}
```

Looks simple.

But what if salary changes require

- Approval

- Audit Log

- Email Notification

- Tax Calculation

- Payroll Update

A property cannot express business intent.

A method can.

Example

```csharp
employee.IncreaseSalary(5000);
```

Inside the method

- Validate amount

- Update salary

- Save history

- Publish event

- Send notification

Methods represent behavior.

Properties represent state.

---

## Rule

Use

Properties

When setting a value is simple.

Use

Methods

When updating data involves business rules.

---

# 17. Best Practices

## 1. Keep Fields Private

✔ Good

```csharp
private decimal balance;
```

---

## 2. Expose Only What is Necessary

Don't expose every field.

Expose only what consumers need.

---

## 3. Validate Before Updating

Never trust external input.

```csharp
if(age < 0)
    throw ...
```

---

## 4. Constructor Should Create Valid Objects

Good

```csharp
Student student =
new Student(1,"Kuldeep",18);
```

Bad

```csharp
Student student =
new Student(-1,"",-5);
```

Never allow invalid objects.

---

## 5. Prefer Behavior Over Data

Bad

```csharp
account.Balance -= 500;
```

Good

```csharp
account.Withdraw(500);
```

---

## 6. Restrict Setters

Instead of

```csharp
public string Name
{
    get;
    set;
}
```

Prefer

```csharp
public string Name
{
    get;
    private set;
}
```

---

# 18. Common Mistakes

## Mistake 1

Public Fields

```csharp
public decimal Balance;
```

---

## Mistake 2

Public Setters Everywhere

```csharp
public decimal Salary
{
    get;
    set;
}
```

---

## Mistake 3

Skipping Validation

Never trust incoming data.

---

## Mistake 4

Objects in Invalid State

```csharp
Student s =
new Student(1,"",-10);
```

Should never happen.

---

## Mistake 5

Using Properties for Business Operations

Bad

```csharp
order.Status = "Delivered";
```

Good

```csharp
order.MarkAsDelivered();
```

---

# 19. Your Student Project

Your Student project follows good encapsulation practices.

### Good Decisions

✔ Private fields

✔ Validation in properties

✔ Constructor creates valid object

✔ Private setters

✔ Methods

```csharp
ChangeStudentCourse()

ChangeStudentAge()
```

instead of

```csharp
student.Course = ...
```

Excellent.

---

## Improvements

Instead of

```csharp
ChangeStudentAge()
```

Consider

```csharp
CelebrateBirthday()
```

or

```csharp
UpdateAge()
```

The method name should represent business intent.

Similarly

Instead of

```csharp
ChangeStudentCourse()
```

You could use

```csharp
EnrollInCourse()
```

if that better reflects the business domain.

Good method names communicate intent, not just the technical action.

---

# 20. Key Takeaways

✔ Properties help implement encapsulation.

✔ Use backing fields when validation or business logic is required.

✔ Auto-properties are suitable for simple data.

✔ Constructors should create valid objects.

✔ Protect object invariants.

✔ Prefer methods over public setters for business operations.

✔ Think in terms of behavior rather than exposing data.


# 🎯 Interview Corner

## Beginner Level

### Q1. What is Encapsulation?

#### Answer

Encapsulation is the process of bundling data (fields) and the methods that operate on that data into a single unit (class) while restricting direct access to the object's internal state.

Its primary goal is to protect an object's state and ensure it remains valid by exposing only controlled operations.

**Keywords to mention**

- Protect object state
- Hide implementation
- Controlled access
- Validation
- Business rules

---

### Q2. Why do we need Encapsulation?

#### Answer

Without encapsulation, any part of the application can modify an object's data directly, making it easy for objects to enter an invalid state.

Encapsulation ensures:

- Data remains valid
- Business rules are enforced
- Internal implementation can change without affecting other classes
- Code becomes easier to maintain

---

### Q3. How do you achieve Encapsulation in C#?

#### Answer

Encapsulation can be achieved using:

- Private fields
- Properties
- Access modifiers
- Methods
- Constructor validation

Example:

```csharp
private decimal balance;

public decimal Balance => balance;

public void Deposit(decimal amount)
{
    if(amount <= 0)
        throw new ArgumentException();

    balance += amount;
}
```

---

### Q4. What is the difference between Encapsulation and Data Hiding?

#### Answer

Data Hiding is the technique of restricting direct access to data.

Encapsulation is the broader design principle of protecting an object's state by controlling how it is accessed and modified.

**Data Hiding helps achieve Encapsulation.**

---

## Intermediate Level

### Q5. Why are public setters considered a design smell?

#### Answer

Public setters allow any class to modify an object's state without validation.

Example:

```csharp
employee.Salary = -10000;
```

This can violate business rules.

Instead expose methods like

```csharp
employee.IncreaseSalary(5000);
```

This allows validation, logging, auditing, and enforcing business rules.

---

### Q6. When should you use a Property and when should you use a Method?

#### Answer

Use a Property when:

- Reading or assigning a simple value
- No business logic exists

Use a Method when:

- Validation is required
- Multiple fields change
- Business rules exist
- Side effects occur (logging, notifications, events)

Example

❌

```csharp
order.Status = Delivered;
```

✔

```csharp
order.MarkAsDelivered();
```

---

### Q7. Why should constructors validate input?

#### Answer

Constructors should ensure that an object is valid immediately after creation.

Without validation:

```csharp
new Student("", -5, "");
```

creates an invalid object.

Validation guarantees object integrity.

---

### Q8. What is an Object Invariant?

#### Answer

An Object Invariant is a condition that must always remain true during an object's lifetime.

Examples

```
Age >= 0

Balance >= 0

Salary >= MinimumSalary
```

Encapsulation protects these invariants.

---

### Q9. Why should fields generally be private?

#### Answer

Private fields:

- Prevent unauthorized modification
- Force updates through validation
- Hide implementation details
- Reduce coupling

---

## Senior Level

### Q10. Can Encapsulation exist without private fields?

#### Answer

Yes.

Private fields are one way to achieve encapsulation.

Encapsulation can also be achieved using:

- Interfaces
- Abstract classes
- Read-only properties
- Methods
- Immutable objects

The goal is controlled access, not simply using the `private` keyword.

---

### Q11. Why is this better?

```csharp
order.MarkAsDelivered();
```

than

```csharp
order.Status = OrderStatus.Delivered;
```

#### Answer

Delivery usually involves business rules.

For example:

- Payment completed?
- Shipment dispatched?
- Inventory updated?
- Delivery time stored?
- Notification sent?

A property setter cannot represent business behavior.

A method can.

---

### Q12. How does Encapsulation improve maintainability?

#### Answer

Business rules exist in one place.

If salary validation changes,

only the Employee class changes.

No other class needs modification.

This reduces bugs and makes refactoring easier.

---

### Q13. How does Encapsulation support the Single Responsibility Principle (SRP)?

#### Answer

Encapsulation keeps an object's state and its related behavior together.

The object becomes responsible for maintaining its own validity instead of relying on external classes.

This improves cohesion and aligns with SRP.

---

### Q14. Is Encapsulation only about hiding data?

#### Answer

No.

Hiding data is only one aspect.

Encapsulation is primarily about protecting object state and exposing meaningful behavior.

The goal is consistency and correctness, not secrecy.

---

## Scenario-Based Questions

### Q15. Review this code.

```csharp
public class Employee
{
    public decimal Salary { get; set; }
}
```

What's wrong?

#### Answer

Problems:

- Anyone can set negative salary
- No validation
- No audit
- No business rules
- Violates encapsulation

Better approach

```csharp
private decimal salary;

public decimal Salary => salary;

public void IncreaseSalary(decimal amount)
{
    if(amount <= 0)
        throw new ArgumentException();

    salary += amount;
}
```

---

### Q16. Where have you used Encapsulation in your projects?

#### Sample Answer

In our backend services, domain entities expose behavior instead of public setters.

For example, instead of allowing any class to update an order status directly, we use methods like `MarkAsShipped()` or `CancelOrder()`. These methods validate business rules, update timestamps, and ensure the entity remains in a valid state.

---

### Q17. When is a public setter acceptable?

#### Answer

Public setters are acceptable for DTOs and simple data-transfer objects where no business logic exists.

Examples:

- Request DTOs
- Response DTOs
- View Models
- Configuration classes
- Options classes

Avoid public setters in Domain Models.

---

### Q18. Why is Encapsulation important in enterprise applications?

#### Answer

Enterprise applications contain complex business rules.

Encapsulation ensures:

- Objects remain valid
- Validation is centralized
- Bugs are reduced
- Maintenance becomes easier
- Internal implementation can evolve without affecting consumers

---

## Quick Revision

- Protect object state
- Hide implementation
- Expose behavior
- Validate every state change
- Constructor creates valid objects
- Prefer methods over setters
- Private fields + controlled methods = Good encapsulation