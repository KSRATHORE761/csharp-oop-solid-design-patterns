using System;

public class Student
{

    private int age;
    private string name;
    private string course;
    public int Id { get; private set; }
    public string Name
    {
        get { return name; }
        private set
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The Name cannot be set as empty or null or does not contains whiteSpaces");
            }
            else
            {
                name = value;
            }
        }
    }
    public int Age
    {
        get { return age; }
        private set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("The Age cannot be in negative");
            }
            else
            {
                age = value;
            }
        }
    }
    public string Course
    {
        get { return course; }
        private set
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The Course cannot be set as empty or null or does not contains whiteSpaces");
            }
            else
            {
                course = value;
            }
        }
    }

    public Student(int Id, string Name, int Age, string Course)
    {
        this.Id = Id;
        this.Name = Name;
        this.Age = Age;
        this.Course = Course;
    }
    public void DisplayStudentDetails()
    {
        Console.WriteLine("------------------------Student Details---------------------------");
        Console.WriteLine($"Student ID:{Id}");
        Console.WriteLine($"Student Name:{Name}");
        Console.WriteLine($"Student Age:{Age}");
        Console.WriteLine($"Student Course:{Course}");
    }
    public void ChangeStudentCourse(string newCourse)
    {
        Course = newCourse;
    }
    public void ChangeStudentAge(int newAge)
    {
        Age = newAge;
    }
}