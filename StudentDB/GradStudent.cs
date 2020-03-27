///////////////////////////////////////////////////////
///TINFO 200 A, Winter 2019
///UWTacoma SET, Chuck Costerella
///2019-03-04 - l6oop - C Sharp Programming Lab 6 - A Student Database App
///This program creates a database for holding student information for a school
///The user can look up a student record to view, edit the record and delete it
///The user will be able to also save any record as well as quit the program
///

////////////////////////////////////////////////
// Change History
// Date     Name        Description
//3/7/19    baint       Added class to be used for inheritance, added constructors to create gradstudent objects

using System;

namespace StudentDB
{
    public class GradStudent : Student
    {
        public string FacultyAdvisor { get; set; }

        public decimal TuitionCredit { get; set; }

        // ctor used by the app to read in data interactively from the user
        // inputs - The student id, first name, last name, email address, the enrollment date and time, advisor name, and tuition credit are all passed to the method
        // postcondition - a gradstudent object is instantiated and initialized correctly
        public GradStudent(int id, string first, string last, string email, DateTime enroll, string advisor, decimal credit)
            : base(id, first, last, email, enroll)
        {
            FacultyAdvisor = advisor;
            TuitionCredit = credit;
        }

        // ctor used by the app to read in data interactively from the user
        // inputs - a first name, last name, email address, advisor name, and tuition credit number are all passed to the method
        // postcondition - a gradstudent object is instantiated and initialized correctly
        public GradStudent(string first, string last, string email, string advisor, decimal credit)
            : base(first, last, email)
        {
            FacultyAdvisor = advisor;
            TuitionCredit = credit;
        }
        // expression-boided method to override tostring 
        // this is the labeled output for human-readable display
        public override string ToString() => base.ToString() +
                                                "\n Faculty Advisor: " + FacultyAdvisor +
                                                "\n Tuition Credit: " + TuitionCredit;
        // expression-boided method to override tostring 
        // this is the labeled output for human-readable display
        public override string ToStringFileFormat() => base.ToString() +
                                                "\n" + FacultyAdvisor +
                                                "\n" + TuitionCredit;
    }
}
