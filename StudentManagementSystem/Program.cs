using System;

class Program
{
    static void Main(string[] args)
    {
        Student s1 = new Student(1,"Kuldeep",18,"B.Tech");
        Student s2 = new Student(2,"Mohit",19,"B.COM");

        // Display Student Details;
        s1.DisplayStudentDetails();
        s2.DisplayStudentDetails();

        // Change Student Course
        s1.ChangeStudentCourse("M.TECH");
        s2.ChangeStudentCourse("MBA");

        // Change Students Age
        s1.ChangeStudentAge(22);
        s2.ChangeStudentAge(22);

        // Display Student Details after course and age change;
        s1.DisplayStudentDetails();
        s2.DisplayStudentDetails();
    }
}