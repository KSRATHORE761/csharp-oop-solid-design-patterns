# Association in C#

## 1. What Is Association?

Association is a relationship between two independent objects where one object knows about, interacts with, or uses another object.

It describes a relationship between objects without necessarily implying ownership.

Examples:
- A doctor treats a patient.
- A customer places an order.
- A teacher teaches students.
- An employee reports to a manager.

Association is a general relationship concept. Aggregation and composition are commonly described as more specific forms of association.

## 2. Association in C#

C# does not have an `association` keyword. We represent association through references, method parameters, properties, and collections.

### Example: Doctor and Patient

```csharp
public class Patient
{
    public string Name { get; }

    public Patient(string name)
    {
        Name = name;
    }
}

public class Doctor
{
    public string Name { get; }

    public Doctor(string name)
    {
        Name = name;
    }

    public void Treat(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);

        Console.WriteLine(
            $"{Name} is treating {patient.Name}");
    }
}
```

Usage:

```csharp
var doctor = new Doctor("Dr. Sharma");
var patient = new Patient("Rahul");

doctor.Treat(patient);
```

The doctor interacts with the patient. The doctor does not own the patient's lifecycle.

In this example, the relationship exists during the method call. A persistent object-level association could instead be represented by a property or collection.

## 3. Types of Association

### A. One-to-One

One object is associated with one other object.

Example: A person has one current passport, assuming that is the application's business rule.

### B. One-to-Many

One object is associated with multiple objects.

Example: One department has multiple employees.

### C. Many-to-Many

Multiple objects can be associated with multiple other objects.

Example: Students enroll in multiple courses, and each course has multiple students.

These cardinalities describe the business relationship. They are not enforced automatically just by declaring C# classes.

## 4. Association vs Aggregation vs Composition

| Concept | Meaning | Example |
|---|---|---|
| Association | Objects interact or know about each other | Doctor treats patient |
| Aggregation | A whole groups independently existing objects | Department groups employees |
| Composition | A whole strongly owns its parts | Order owns its order lines |

**Remember:** Association is the broadest concept. Aggregation and composition add meaning about the whole-part relationship and ownership or lifecycle.

## 5. Association Through Different C# Designs

Association can be represented in different ways.

### Method parameter

```csharp
public void Treat(Patient patient)
{
    // Use the patient for this operation.
}
```

### Object reference

```csharp
public class Order
{
    public Customer Customer { get; }

    public Order(Customer customer)
    {
        Customer = customer;
    }
}
```

### Collection

```csharp
public class Teacher
{
    private readonly List<Student> _students = new();

    public void AssignStudent(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);
        _students.Add(student);
    }
}
```

The appropriate representation depends on whether the relationship needs to be retained, queried, navigated, or managed over time.

## 6. Interview Questions and Answers

### Q1. What is association in OOP?

**Answer:** Association is a relationship in which objects interact with or know about one another. It does not inherently imply ownership or shared lifecycle.

### Q2. Is association a C# keyword?

**Answer:** No. It is an object-oriented modeling concept represented using references, properties, collections, and method parameters.

### Q3. What is the difference between association and aggregation?

**Answer:** Association is the general relationship between objects. Aggregation is a more specific whole-part relationship in which the related objects can exist independently of the whole.

### Q4. Can association be unidirectional or bidirectional?

**Answer:** Yes. In a unidirectional association, one object can navigate to the other. In a bidirectional association, both objects maintain a way to navigate to each other. Bidirectional references require careful consistency management.

### Q5. Can association be one-to-many or many-to-many?

**Answer:** Yes. Association can have different cardinalities depending on the domain requirements.

### Q6. Does association imply dependency injection?

**Answer:** No. Association describes a relationship between objects. Dependency injection is a technique for supplying dependencies to an object.

## 7. Senior-Level Scenario Questions

### Scenario 1: Students enroll in multiple courses, and each course contains multiple students. How would you model this?

**Suggested answer:** Model a many-to-many relationship. In a relational database, this is commonly implemented using a junction entity or table such as `StudentCourse`. If enrollment has attributes such as enrollment date or grade, make it an explicit entity.

### Scenario 2: A developer creates bidirectional references between two classes. What problems can arise?

**Suggested answer:** The references can become inconsistent if only one side is updated. Bidirectional navigation can also introduce serialization cycles or complicate object mapping. Keep the relationship as simple as the use cases allow.

### Scenario 3: An order uses a customer to validate eligibility. Is this composition?

**Suggested answer:** Not necessarily. The order interacting with a customer is an association. The customer normally exists independently of the order, so the relationship does not imply strong ownership.

### Scenario 4: How would you represent a manager and their direct reports?

**Suggested answer:** A manager is typically an employee, and the reporting relationship can be modeled using an employee reference or a collection of reports. Define rules for preventing invalid cycles and handling transfers.

### Scenario 5: Is a method parameter enough to establish association?

**Suggested answer:** It can represent a temporary interaction between objects. Whether it constitutes a persistent domain association depends on the domain model and whether the relationship must be retained beyond the operation.

## 8. Common Mistakes

- Treating every association as composition.
- Assuming association automatically means ownership.
- Adding bidirectional navigation without a real requirement.
- Confusing association with dependency injection.
- Ignoring cardinality and consistency rules.
- Assuming a C# reference alone enforces business constraints.

## 9. Quick Revision

- Association means objects interact with or know about each other.
- It can be unidirectional or bidirectional.
- It can be one-to-one, one-to-many, or many-to-many.
- Aggregation and composition describe more specific whole-part relationships.
- Model relationships based on business requirements and lifecycle rules.