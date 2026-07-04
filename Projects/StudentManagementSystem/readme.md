# Student Management System

## 📌 Project Objective

The purpose of this project is to practice fundamental Object-Oriented Programming (OOP) concepts in C# and understand how to design a class that protects its own state using encapsulation.

This project focuses on creating a `Student` object with proper validation, controlled access to data, and meaningful behaviors.

---

## 🧠 OOP Concepts Practiced

- Classes & Objects
- Constructors
- Properties
- Getters & Setters
- Access Modifiers (`public`, `private`)
- Data Hiding
- Encapsulation
- Input Validation
- Methods as Object Behavior

---

## 📋 Problem Statement

Create a Student Management System that allows:

- Creating student objects with valid details.
- Viewing student information.
- Updating a student's course.
- Updating a student's age.
- Preventing invalid student data from being stored.

---

## 🏗 Class Design

### Student

The `Student` class represents a student and contains both data (state) and behavior (methods).

### State (Properties)

| Property | Description | Access Control |
|----------|-------------|----------------|
| Id | Unique student identifier | Read-only outside the class |
| Name | Student's name | Validated before setting |
| Age | Student's age | Validated to prevent negative values |
| Course | Student's enrolled course | Can only be modified through class methods |

---

## ⚙️ Behaviors (Methods)

### DisplayStudentDetails()

Displays complete student information.

### ChangeStudentCourse(string newCourse)

Updates the student's course after validation.

### ChangeStudentAge(int newAge)

Updates the student's age while maintaining validation rules.

---

## 🔒 Encapsulation Decisions

The class is designed to protect its internal state:

- Private fields are used to hide internal data.
- Properties expose controlled access.
- `Id` cannot be changed after object creation.
- Invalid names, ages, and course details are not allowed.
- Data modifications happen through class behavior rather than direct access.

---

## 💻 Sample Usage

```csharp
Student student = new Student(
    1,
    "Kuldeep",
    18,
    "B.Tech"
);

student.DisplayStudentDetails();

student.ChangeStudentCourse("M.Tech");

student.ChangeStudentAge(22);
```

---

## 📂 Project Structure

```
StudentManagementSystem
|
|-- Program.cs     // Application entry point
|-- Student.cs     // Student class containing data and behavior
```

---

## 🚀 Learning Outcome

By completing this project, I learned how to:

- Design classes using real-world objects.
- Differentiate between state and behavior.
- Use constructors to create valid objects.
- Apply access modifiers to protect data.
- Implement encapsulation using properties and methods.
- Think about object design rather than only writing code.

---

## 🔄 Future Improvements

Possible enhancements for this project:

- Add multiple students using a collection.
- Create a separate `StudentService` class to manage students.
- Add search functionality using student ID.
- Implement file or database storage.
- Create a menu-driven console application.

---

## 📚 Part of Learning Journey

This project is part of my journey to master:

```
C# OOP → SOLID Principles → Design Patterns → Low-Level Design
```
