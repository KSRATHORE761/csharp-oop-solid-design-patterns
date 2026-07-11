# Vehicle Management System

A C# Object-Oriented Programming (OOP) project demonstrating the core principles of OOP using a Vehicle Management System.

## Features

### Vehicle (Base Class)

- Start vehicle
- Stop vehicle
- Accelerate
- Brake
- Display vehicle details

### Car

- Open trunk
- Close trunk
- Honk
- Display car-specific details

### Bike

- Put side stand down
- Lift side stand
- Perform wheelie
- Display bike-specific details

### Electric Car

- Open charging port
- Close charging port
- Charge battery
- Drive in Eco Mode
- Display electric car details

---

# OOP Concepts Covered

## Encapsulation

- Private setters
- Data validation through constructors
- Controlled state changes through methods

Examples:
- Vehicle cannot stop while moving.
- Speed cannot exceed maximum limit.
- Battery percentage remains within valid range.

---

## Inheritance

```
Vehicle
│
├── Car
│      │
│      └── ElectricCar
│
└── Bike
```

- Car inherits Vehicle.
- Bike inherits Vehicle.
- ElectricCar inherits Car.

---

## Polymorphism

### Method Overriding

Overridden methods:

- Start()
- DisplayVehicleDetails()

Each vehicle provides its own implementation.

Example:

- Car → Starting Petrol Car...
- Bike → Checking Side Stand...
- ElectricCar → Checking Battery...

---

### Runtime Polymorphism

Objects are stored as:

```csharp
List<Vehicle>
```

Calling

```csharp
vehicle.Start();
```

executes the correct overridden method based on the runtime type.

---

### Upcasting

```csharp
Vehicle vehicle = new Car(...);
```

---

### Downcasting

```csharp
Car car = (Car)vehicle;
```

---

### is Operator

Used for runtime type checking before explicit casting.

Example:

```csharp
if(vehicle is Car)
```

---

### as Operator

Safe casting without throwing exceptions.

Example:

```csharp
Car car = vehicle as Car;
```

---

### Pattern Matching

Modern C# pattern matching:

```csharp
if(vehicle is Car car)
{
    car.Honk();
}
```

---

### Dynamic Method Dispatch

Demonstrates how overridden methods are selected at runtime based on the actual object type.

---

# Validation Rules

Vehicle

- Vehicle Id must be positive.
- Brand cannot be empty.
- Model cannot be empty.
- Year must be within the allowed range.
- Vehicle cannot accelerate before starting.
- Vehicle cannot stop while moving.

Car

- Trunk cannot be opened while the car is moving.
- Honk works only when the vehicle is started.

Bike

- Side stand cannot be lowered while moving.
- Wheelie requires:
  - Bike started
  - Speed ≥ 25 km/h

Electric Car

- Charging port can only be opened when the vehicle is stopped.
- Battery charging requires:
  - Charging port open
  - Vehicle stopped
- Battery percentage cannot exceed 100%.

---

# Technologies Used

- C#
- .NET
- Object-Oriented Programming

---

# Learning Outcomes

This project demonstrates:

- Encapsulation
- Inheritance
- Runtime Polymorphism
- Method Overriding
- Upcasting
- Downcasting
- is Operator
- as Operator
- Pattern Matching
- Dynamic Method Dispatch
- Exception Handling
- Constructor Validation

---

# Project Structure

```
VehicleManagementSystem
│
├── Vehicle.cs
├── Car.cs
├── Bike.cs
├── ElectricCar.cs
└── Program.cs
```

---

# Future Improvements

- Abstraction using abstract classes
- Interfaces
- Composition over Inheritance
- Exception logging
- Unit Testing
- Dependency Injection