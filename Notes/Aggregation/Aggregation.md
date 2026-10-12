# Aggregation in C#

## 1. What Is Aggregation?

Aggregation is a form of association in which one object refers to or groups other objects that can exist independently of it.

It is commonly described as a **weak has-a relationship**.

Example:
- A department has employees.
- A team has players.
- A library has members.

An employee can exist even if a particular department is removed. The relationship does not imply that the employee's lifecycle must end with the department's lifecycle.

**Important:** C# does not have a special `aggregation` keyword. Aggregation is a design concept represented through object references and application rules.

## 2. Basic Example

```csharp
public class Employee
{
    public int Id { get; }
    public string Name { get; }

    public Employee(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

public class Department
{
    private readonly List<Employee> _employees = new();

    public string Name { get; }

    public Department(string name)
    {
        Name = name;
    }

    public void AddEmployee(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);
        _employees.Add(employee);
    }

    public IReadOnlyList<Employee> Employees => _employees;
}
```

Usage:

```csharp
var employee = new Employee(101, "Amit");

var department = new Department("Engineering");
department.AddEmployee(employee);

Console.WriteLine(department.Employees[0].Name);
```

The employee is created independently and then associated with the department.

The `Department` does not create the `Employee` in this example. However, creation location alone does not determine whether a relationship is aggregation or composition; lifecycle and ownership rules matter too.

## 3. Characteristics of Aggregation

- Objects have an association or grouping relationship.
- The related objects can exist independently.
- Removing the association does not necessarily destroy either object.
- The same object may participate in other relationships, if business rules permit.
- Ownership and lifecycle rules are determined by the application design.

## 4. Aggregation vs Association

**Association** is the broader concept: one object knows about or works with another object.

**Aggregation** is a more specific conceptual relationship in which one object groups or refers to independently existing objects.

For example, a doctor treating a patient is an association. A department grouping employees is commonly modeled as aggregation.

These distinctions depend on the domain model; C# syntax itself does not enforce them.

## 5. Business Scenario: Employees Moving Between Departments

An employee moves from Engineering to Finance.

The employee remains the same person with the same identity. The application changes the department relationship instead of deleting and recreating the employee.

A simplified model could provide methods such as:

```csharp
public void RemoveEmployee(Employee employee)
{
    _employees.Remove(employee);
}
```

In a real application, the transfer should be coordinated by an appropriate service so that both departments remain consistent and the employee is not accidentally assigned to conflicting departments.

## 6. Aggregation and Database Relationships

Aggregation is not the same as a foreign key, but the concepts can work together.

For example, an `Employees` table may contain a nullable `DepartmentId` foreign key.

- An employee can exist without a department if the business permits it.
- A department may group many employees.
- Removing a department should follow an explicit business and database policy: reassign employees, set the department reference to null, or reject the operation.

Never assume that a database cascade-delete rule is automatically appropriate just because objects have a has-a relationship.

## 7. Interview Questions and Answers

### Q1. What is aggregation?

**Answer:** Aggregation is a conceptual has-a relationship in which the contained or grouped objects can exist independently of the containing object.

### Q2. Does C# provide an aggregation keyword?

**Answer:** No. Aggregation is modeled through ordinary classes, references, collections, and application rules.

### Q3. What is the difference between aggregation and association?

**Answer:** Association is the general relationship between objects. Aggregation is a more specific grouping relationship in which the related objects have independent lifecycles.

### Q4. Can an aggregated object be shared?

**Answer:** Yes, when the business model permits it. However, sharing an object does not by itself prove that the relationship is aggregation.

### Q5. If a department is deleted, must its employees be deleted?

**Answer:** No. In an aggregation-style model, employees can continue to exist independently. The application must decide whether to reassign them, remove the association, or prevent department deletion.

### Q6. Is aggregation enforced by the compiler?

**Answer:** No. C# does not enforce aggregation semantics. Developers must model and enforce the intended rules.

## 8. Senior-Level Scenario Questions

### Scenario 1: A department is deleted while employees still belong to it. What should happen?

**Suggested answer:** First clarify the business rule. The system might prevent deletion while employees exist, reassign them, or remove their department association. The database constraints and application behavior must agree.

### Scenario 2: Can one employee belong to multiple departments?

**Suggested answer:** That depends on the business requirement. If multiple assignments are allowed, model them explicitly, potentially using an employee-department assignment entity with role, dates, and status.

### Scenario 3: How would you model a department hierarchy with employees and managers?

**Suggested answer:** Separate employee identity from department membership and reporting relationships. Define constraints for manager assignments, department transfers, and cycles in the hierarchy rather than relying only on object references.

### Scenario 4: Is a collection of objects always aggregation?

**Suggested answer:** No. A collection can represent aggregation, composition, or another relationship. Lifecycle, ownership, and business rules determine the model.

### Scenario 5: How do you ensure that deleting a department does not accidentally delete employees in EF Core?

**Suggested answer:** Configure the entity relationship and delete behavior deliberately, inspect the generated schema, and test deletion scenarios. Do not rely on assumptions about default cascade behavior.

## 9. Common Mistakes

- Treating aggregation as a special C# language feature.
- Assuming every collection represents aggregation.
- Assuming object creation location alone determines ownership.
- Deleting independently managed entities automatically.
- Confusing aggregation with a database foreign key.
- Ignoring transfer rules and referential integrity.

## 10. Quick Revision

- Aggregation is commonly a weak has-a relationship.
- The related objects have independent lifecycles.
- C# does not enforce aggregation directly.
- Business rules determine how relationships change when an object is removed.
- Database constraints and application rules must remain consistent.