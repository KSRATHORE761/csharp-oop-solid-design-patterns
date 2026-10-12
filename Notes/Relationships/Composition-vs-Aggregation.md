# Composition vs Aggregation in C#

## 1. Overview

Composition and aggregation are both forms of whole-part relationships.

The key distinction is the intended **ownership and lifecycle relationship** between the whole and its parts.

- **Composition:** Strong ownership; a part is conceptually owned by the whole and generally does not have an independent lifecycle in that relationship.
- **Aggregation:** Weaker grouping; the related objects can exist independently of the whole.

These are modeling concepts, not special C# language features. Real application designs may have more nuanced lifecycle rules.

## 2. Key Differences

| Feature | Composition | Aggregation |
|---|---|---|
| Relationship | Strong whole-part ownership | Weak whole-part grouping |
| Lifecycle | Part is commonly managed as part of the whole | Part can exist independently |
| Independent reuse | Often not intended outside its owner | Usually possible when business rules allow |
| Object creation | Often managed by the owner | Often created or managed independently |
| Example | An order and its order lines | A department and its employees |
| C# enforcement | Not enforced automatically | Not enforced automatically |

Do not use object creation location as the only criterion. A constructor can create a dependency in either design; the domain's ownership and lifecycle rules are more important.

## 3. Composition Example: Order and Order Items

In this simplified model, order items are created and managed through the order. They have no public constructor and cannot be added directly by outside callers.

```csharp
public class Order
{
    private readonly List<OrderItem> _items = new();

    public IReadOnlyList<OrderItem> Items => _items;

    public void AddItem(string product, int quantity)
    {
        if (string.IsNullOrWhiteSpace(product))
            throw new ArgumentException("Product is required.");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        _items.Add(new OrderItem(product, quantity));
    }

    private sealed class OrderItem
    {
        public string Product { get; }
        public int Quantity { get; }

        public OrderItem(string product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}
```

This design expresses the rule that an order manages its own items.

In a production application, order items may have database identities and may be persisted through EF Core. The domain relationship should still be defined by the business rules, not merely by whether the database uses separate rows.

## 4. Aggregation Example: Department and Employees

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

    public void AddEmployee(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);
        _employees.Add(employee);
    }

    public IReadOnlyList<Employee> Employees => _employees;
}
```

An employee can exist independently of a department. The department groups employee objects but does not necessarily own their entire lifecycle.

## 5. How to Decide in a Business Application

Ask these questions:

1. Can the part exist independently of the whole?
2. Can the part be transferred to another whole?
3. Who creates, updates, and removes the part?
4. What should happen to the part when the whole is deleted?
5. Do database and domain rules reflect the same lifecycle expectations?

For example, an order line is usually tightly tied to its order, while an employee's identity and employment record normally outlive a particular department assignment.

## 6. Interview Questions and Answers

### Q1. What is the main difference between composition and aggregation?

**Answer:** Composition expresses strong whole-part ownership, whereas aggregation expresses a weaker relationship in which the related objects can exist independently of the whole.

### Q2. Is composition stronger than aggregation in C# syntax?

**Answer:** No. C# has no dedicated syntax for either concept. The strength of the relationship comes from the domain model, access restrictions, lifecycle management, and application rules.

### Q3. Can an object be transferred from one owner to another?

**Answer:** It depends on the domain. Independently managed objects, such as employees, can commonly be reassigned. A tightly owned part, such as an order line, generally must follow the rules of its order.

### Q4. Is a constructor-created object always composition?

**Answer:** No. The fact that one object creates another does not fully define the relationship. Consider whether the part has an independent identity, lifecycle, or ownership in the domain.

### Q5. Can composition and aggregation both use collections?

**Answer:** Yes. A collection can represent either relationship. Its meaning depends on ownership and lifecycle requirements.

### Q6. How does this distinction affect deletion?

**Answer:** In composition, deleting the whole commonly removes its owned parts. In aggregation, the related objects can survive, but their association may need to be removed or reassigned. The actual deletion policy must be implemented explicitly.

## 7. Senior-Level Scenario Questions

### Scenario 1: An order is cancelled. What should happen to its order items?

**Suggested answer:** Usually, the items remain part of the order's historical record, even if the order can no longer be fulfilled. Composition does not necessarily mean physical deletion; the business may use status changes, soft deletion, or retention rules.

### Scenario 2: A department is deleted, but employees must remain in the system.

**Suggested answer:** Model employees independently and define what happens to department membership. Reassignment, unassignment, or blocking deletion are possible policies.

### Scenario 3: A developer uses cascade delete for every parent-child relationship. Is that safe?

**Suggested answer:** No. Cascade delete can be correct for tightly owned dependents, but it can destroy independently managed data. Review domain ownership, foreign keys, retention requirements, and deletion behavior for each relationship.

### Scenario 4: A child record has its own database ID. Does that mean aggregation?

**Suggested answer:** No. Database identity does not determine the conceptual relationship. A composition-owned entity can have its own primary key and still be governed by the lifecycle of its aggregate or owner.

### Scenario 5: How would you test the distinction?

**Suggested answer:** Write tests for the intended business lifecycle: adding and removing parts, transferring independent entities, handling deletion, and enforcing ownership boundaries. For persistence behavior, add integration tests.

## 8. Common Mistakes

- Defining composition only by who calls `new`.
- Assuming every parent-child relationship is composition.
- Assuming composition requires physical deletion of child records.
- Using cascade delete without considering data retention.
- Assuming database IDs determine ownership.
- Ignoring business rules for transfers and deletion.

## 9. Quick Revision

- Composition: strong whole-part ownership.
- Aggregation: weaker grouping of independently existing objects.
- Both are conceptual relationships rather than C# keywords.
- Lifecycle and business rules are more important than syntax.
- Test the behavior when parts are added, removed, transferred, or retained.