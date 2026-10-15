# Chapter 1: Classes and Objects in C#

Classes and objects represent **real-world entities and concepts**.

- A **class** is a blueprint used to create objects with similar properties and behaviour.
- An **object** is an instance of a class.

**Example:** `Dog` is a class, while a specific dog like `Tommy` is an object of that class.

```
Class: Dog  ──── create instance ────▶  Object: Dog "Tommy"
```

---

## Classes

A class is a **user-defined data type** that encapsulates data and behaviour. It can contain fields, properties, methods, constructors, and events.

A class itself does not occupy memory until its objects are created.

### Syntax

```csharp
class ClassName
{
    // Fields
    // Properties
    // Methods
}
```

### Class blueprint in C#

| Part | Maps to |
|------|---------|
| **Identity** | Name of the class |
| **State / Attribute** | Fields / Properties |
| **Behaviour** | Methods (functions) |

### Example

```csharp
public class Geeks
{
    public int a, b;            // fields (variables)

    public void Display()
    {
        Console.WriteLine("Class in C#");
    }
}
```

---

## Objects

An object is an instance of a class. It represents a real-world entity.

An object consists of:

1. **State (Data):** represented by the attributes of an object; reflects the properties of the object.
2. **Behaviour:** represented by the methods of an object; reflects how an object interacts with other objects.
3. **Identity:** represents the unique reference of an object.

> **Note:** Classes define the blueprint of state, behaviour and identity. Objects are the entities that actually hold the state, show behaviour and carry identity.

### Declaration and initialization of an object

```csharp
Car myCar;              // Declaration: creates a reference variable of type Car
myCar = new Car();      // Initialization: allocates memory using the 'new' keyword
```

Both steps combined:

```csharp
Car myCar = new Car();
//  ↑ Declaration   ↑ Initialization
```

### Full example

```csharp
using System;

public class Car
{
    // State: fields that hold data of an object
    public string Color;
    public int Speed;

    // Behaviour: method that defines what the object can do
    public void Drive(int speedIncrease)
    {
        Speed += speedIncrease;
        Console.WriteLine($"{Color} car is driving at {Speed} km/h");
    }

    static void Main(string[] args)
    {
        // Object declaration & creation
        Car car1 = new Car();
        Car car2 = new Car();

        // Setting state
        car1.Color = "Red";
        car1.Speed = 60;

        car2.Color = "Red";
        car2.Speed = 60;

        // Triggering behaviour
        car1.Drive(20);
        car2.Drive(10);
    }
}
```

**Output:**

```
Red car is driving at 80 km/h
Red car is driving at 70 km/h
```

Each object has its own state: `car1` and `car2` are separate objects with separate `Speed` values.

---

## Quick Recap

- Class = blueprint, Object = instance.
- `new` allocates the object and returns a reference.
- Object = **State + Behaviour + Identity**.