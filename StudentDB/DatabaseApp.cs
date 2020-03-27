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
//2/19/19   baint       Database program created, methods to read and write data were created
//2/21/19   baint       Methods to display main menu as well as get user selection were created
//2/26/19   baint       Started created CRUD methods
//2/28/19   baint       Added create method as well as update method
//3/5/19    baint       Added to create and update method to get them working 
//3/7/19    baint       Added inheritance for Undergrad and Grad students

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDB
{
    class DatabaseApp
    {
        public const bool DebugTestMode = false;
        public const int NotFound = -1;


        //actual storage for the students
        private List<Student> students = new List<Student>();

        static void Main(string[] args)
        {
            DatabaseApp dbApp = new DatabaseApp();
        }
        public DatabaseApp()
        {
            GoDatabase();
        }

        // Function - Starts and runs the program
        // Postconditions - The user selected what they want to do to the data and the corresponding method has been called
        private void GoDatabase()
        {
            //just unitl we can get input file working
            if (DebugTestMode) TestMain();
            ReadDataFromInputFile();

            while (true)
            {
                //display the main menu to the user
                DisplayMainMenu();

                //get a selection from the user
                ConsoleKeyInfo selection = GetSelectionFromUser();
                //use that selection in a switch statement to ececute a CRUD operation
                switch (selection.KeyChar)
                {
                    case 'P':
                    case 'p':
                        PrintAllRecords();
                        break;
                    case 'C':
                    case 'c':
                        CreateNewStudentRecord();
                        break;
                    case 'F':
                    case 'f':
                        FindStudentRecord();
                        break;
                    case 'U':
                    case 'u':
                        UpdateStudentRecord();
                        break;
                    case 'D':
                    case 'd':
                        DeleteStudentRecord();
                        break;
                    case 'S':
                    case 's':
                        //this is to save without exiting app
                        WriteDataToOutputFile();
                        break;
                    case 'Q':
                    case 'q':
                        QuitDatabaseApp();
                        break;
                    default:
                        break;
                }
            }
        }

        // Function - Finds and displays a student record
        // Precondidtions - There is a student that is able to be found 
        // Postconditions - The student record was printed
        private void FindStudentRecord()
        {
            int location = GetIndexFromList();

            Console.WriteLine($"Student    ID:    {students[location].StudentID} (id is read only)");
            Console.WriteLine($"[F]irst  name:    {students[location].FirstName}");
            Console.WriteLine($"[L]ast   name:    {students[location].LastName}");
            Console.WriteLine($"[E]mail addre:    {students[location].EmailAddress}");
            Console.WriteLine($"[D]ate Enrolled:    {students[location].EnrollmentDate}");
            if (students[location] is GradStudent)
            {
                GradStudent grad = (GradStudent)students[location];
 
                Console.WriteLine($"Faculty [A]dvisor:   {grad.FacultyAdvisor}");
                Console.WriteLine($"[T]uition Credit :   {grad.TuitionCredit}");
            }
            else
            {
                Undergrad under = (Undergrad)students[location];

                Console.WriteLine($"Year Rank:     {under.Rank}");
                Console.WriteLine($"GPA      :     {under.GradePointAverage}");
            }
        }

        // Function - creates a new record and stores it in the database
        // Postconditions - A new record is created and saved to database
        private void CreateNewStudentRecord()
        {
            Console.WriteLine("\nCreating a new Student Record.");
            string email = string.Empty;
            bool emailNotAvailable = false;

            Console.Write("Is the record a Grad Student or an Undergrad(G / U): ");
            string studentType = Console.ReadLine();
            {
                do
                {
                    Console.WriteLine("Choose an email address (must be system unique): ");
                    email = Console.ReadLine();

                    for (int i = 0; i < students.Count; i++)
                    {
                        if (students[i].EmailAddress == email)
                        {
                            emailNotAvailable = true;
                            Console.WriteLine($"Sorry, {email} is in use - it is not available.");
                            break;
                        }
                    }
                } while (emailNotAvailable);

                //rest of the student create
                Console.Write("First Name: ");
                string first = Console.ReadLine();

                Console.Write("Last Name: ");
                string last = Console.ReadLine();

                DateTime enrollment = DateTime.Now;

                if (studentType == "G" || studentType == "g")
                {

                    Console.Write("Faculty Advisor: ");
                    string advisor = Console.ReadLine();

                    Console.Write("Tuition Credit: ");
                    decimal credit = decimal.Parse(Console.ReadLine());

                    students.Add(new GradStudent(students.Count + 1, first, last, email, enrollment, advisor, credit));
                }
                else
                {
                    Console.Write("Year Rank [1, 2, 3, or 4 only]: ");
                    int yearRank = int.Parse(Console.ReadLine());

                    Console.Write("Grade Point Average: ");
                    float gpa = float.Parse(Console.ReadLine());

                    if (yearRank == 1) students.Add(new Undergrad(students.Count + 1, first, last, email, enrollment, YearRank.Freshman, gpa));
                    if (yearRank == 2) students.Add(new Undergrad(students.Count + 1, first, last, email, enrollment, YearRank.Sophomore, gpa));
                    if (yearRank == 3) students.Add(new Undergrad(students.Count + 1, first, last, email, enrollment, YearRank.Junior, gpa));
                    if (yearRank == 4) students.Add(new Undergrad(students.Count + 1, first, last, email, enrollment, YearRank.Senior, gpa));
                }
            }
        }

        // Function - lets the user find a record and then update whatever part they wish to, saving the update
        // Precondidtions - There is a student record to update
        private void UpdateStudentRecord()
        {
            Console.WriteLine("Attempting to update student record");

            //assume that the value entered is absolutely within range
            int location = GetIndexFromList();

            // RTTI to determine what kind of student
            if (students[location] is Undergrad)
            {
                EditUndergradRecord(location);
            }
            else
            {
                EditGradStudentRecord(location);
            }
        }

        // Function - Takes an undergrad record, edits it, and saves it
        // Precondidtions - There is an undergrad record to edit
        // Inputs - The int location is passed in to this method so it knows where to look to edit
        // Postconditions - the edited record is updated and saved
        private void EditUndergradRecord(int location)
        {
            Undergrad under = (Undergrad)students[location];

            Console.WriteLine($"  Student    ID:    {under.StudentID} (**readonly field**)");
            Console.WriteLine($"  [F]irst  name:    {under.FirstName}");
            Console.WriteLine($"  [L]ast   name:    {under.LastName}");
            Console.WriteLine($"  [E]mail addre:    {under.EmailAddress} (**readonly field**)");
            Console.WriteLine($"[D]ate Enrolled:    {under.EnrollmentDate}");
            Console.WriteLine($"    [Y]ear Rank:    {under.Rank}");
            Console.WriteLine($"[G]rade Pt. Avg:    {under.GradePointAverage}");

            char selection = char.Parse(Console.ReadLine());

            switch (selection)
            {
                case 'F':
                case 'f':
                    Console.WriteLine("Enter the new first name: ");
                    students[location].FirstName = Console.ReadLine();
                    break;
                case 'L':
                case 'l':
                    Console.WriteLine("Enter the new last name: ");
                    students[location].LastName = Console.ReadLine();
                    break;
                case 'Y':
                case 'y':
                    Console.WriteLine("Enter the new year rank in school [1, 2, 3, or 4 only]: ");
                    int yearRank = int.Parse(Console.ReadLine());
                    if (YearRank.Freshman.Equals(yearRank)) under.Rank = YearRank.Freshman;
                    if (YearRank.Sophomore.Equals(yearRank)) under.Rank = YearRank.Sophomore;
                    if (YearRank.Junior.Equals(yearRank)) under.Rank = YearRank.Junior;
                    if (YearRank.Senior.Equals(yearRank)) under.Rank = YearRank.Senior;
                    break;
                case 'G':
                case 'g':
                    Console.WriteLine("Enter the new GPA");
                    under.GradePointAverage = float.Parse(Console.ReadLine());
                    break;
                default:
                    break;
            }
        }

        // Function - (when complete) take a grad student record, edit it, and save 
        // Precondidtions - There is a grad student record to edit
        // Inputs - The int location is passed in so that the program knows where to look to edit
        // Postconditions - the grad student record is changed and saved
        private void EditGradStudentRecord(int location)
        {
            GradStudent grad = (GradStudent)students[location];

            Console.WriteLine($"    Student    ID:    {grad.StudentID} (**readonly field**)");
            Console.WriteLine($"    [F]irst  name:    {grad.FirstName}");
            Console.WriteLine($"    [L]ast   name:    {grad.LastName}");
            Console.WriteLine($"    [E]mail addre:    {grad.EmailAddress} (**readonly field**)");
            Console.WriteLine($"  [D]ate Enrolled:    {grad.EnrollmentDate}");
            Console.WriteLine($"Faculty [A]dvisor:    {grad.FacultyAdvisor}");
            Console.WriteLine($" [T]uition Credit:    {grad.TuitionCredit}");

            char selection = char.Parse(Console.ReadLine());

            switch (selection)
            {
                case 'F':
                case 'f':
                    Console.WriteLine("Enter the new first name: ");
                    students[location].FirstName = Console.ReadLine();
                    break;
                case 'L':
                case 'l':
                    Console.WriteLine("Enter the new last name: ");
                    students[location].LastName = Console.ReadLine();
                    break;
                case 'A':
                case 'a':
                    Console.WriteLine("Enter the new Faculty Advisor: ");
                    string facultyAdvisor = Console.ReadLine();
                    grad.FacultyAdvisor = facultyAdvisor;
                    
                    break;
                case 'T':
                case 't':
                    Console.WriteLine("Enter the new tuition credit: ");
                    grad.TuitionCredit = decimal.Parse(Console.ReadLine());
                    break;
                default:
                    break;
            }
        }

        // Function - deletes a record in the list if it exists
        // Precondidtions - There is a record to be deleted
        // Postconditions - after returning the record is not in the list
        private void DeleteStudentRecord()
        {
            //prompt the user for the record to be deleted
            //check the list<> to see if its in there
            int location = GetIndexFromList();

            if (location == NotFound)
            {
                Console.WriteLine("Record not found. Can't delete.");
            }
            else
            {
                //if it is in the list,. - remove the record from the list
                Console.WriteLine($"Removing {students[location].FirstName} {students[location].LastName}");
                students.RemoveAt(location);
                Console.WriteLine("....Done");
            }
        }

        // Function - Lets the user select a record to look at/edit from a list of emails
        // Precondidtions - There are reocrds to select from
        // Outputs - The location of the chosen email is returned so that the chosen record can be acted upon.
        private int GetIndexFromList()
        {
            Console.WriteLine();
            Console.WriteLine("*********************************");
            Console.WriteLine("** Student email address list **");
            Console.WriteLine("*********************************");

            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i} :: {students[i].EmailAddress}");
            }
            Console.Write("SELECT email: ");
            int location = int.Parse(Console.ReadLine());

            return location;
        }

        // Function - Takes the input file, reads it, and stores the data
        // Precondidtions - The input file has data to store, and the data is in the correct format
        // Postconditions - The data is stored as variables within the program
        private void ReadDataFromInputFile()
        {
            // construct an object connected to the input file
            StreamReader infile = new StreamReader("STUDENT_DATABASE_INPUT_FILE.txt");

            string str = string.Empty;
            // read the data - and store it in the list<>
            while ((str = infile.ReadLine()) != null)
            {
                int id = int.Parse(str);
                string first = infile.ReadLine();
                string last = infile.ReadLine();
                string email = infile.ReadLine();
                DateTime enrolled = DateTime.Parse(infile.ReadLine());

                //assumes the value is the year rank, if it is not, the advisor variable is assigned the rank value
                string rank = infile.ReadLine();

                if (rank == "Freshman" || rank == "Sophomore" || rank == "Junior" || rank == "Senior")
                {
              
                    float gpa = float.Parse(infile.ReadLine());

                    if (rank == "Freshman")
                    {
                        Undergrad under = new Undergrad(id, first, last, email, enrolled, YearRank.Freshman, gpa);
                        students.Add(new Undergrad(id, first, last, email, enrolled, YearRank.Freshman, gpa));
                    }
                    else if (rank == "Sophomore")
                    {
                        Undergrad under = new Undergrad(id, first, last, email, enrolled, YearRank.Sophomore, gpa);
                        students.Add(new Undergrad(id, first, last, email, enrolled, YearRank.Sophomore, gpa));
                    }
                    else if (rank == "Junior")
                    {
                        Undergrad under = new Undergrad(id, first, last, email, enrolled, YearRank.Junior, gpa);
                        students.Add(new Undergrad(id, first, last, email, enrolled, YearRank.Junior, gpa));
                    }
                    else
                    {
                        Undergrad under = new Undergrad(id, first, last, email, enrolled, YearRank.Senior, gpa);
                        students.Add(new Undergrad(id, first, last, email, enrolled, YearRank.Senior, gpa));
                    }
                }
                else
                {
                    string advisor = rank;
                    decimal credit = decimal.Parse(infile.ReadLine());
                    GradStudent grad = new GradStudent(id, first, last, email, enrolled, advisor, credit);
                    students.Add(new GradStudent(id, first, last, email, enrolled, advisor, credit));
                }


                //we should have enough data for a complete record
                // and we can make a student
                //Student stu = new Student(id, first, last, email, enrolled);

                //put the new student in the list<>
                //students.Add(new Student(id, first, last, email, enrolled));
            }
            //close the file
            infile.Close();
        }

        // Function - Prints every record that is stored
        // Precondidtions - There are records to print
        // Postconditions - The records are displayed to the user
        private void PrintAllRecords()
        {
            Console.WriteLine();
            for (int i = 0; i < students.Count; ++i)
            {
                Console.WriteLine(students[i]);
            }
        }

        // Function - Exits out of the program
        // Precondidtions - The program is running
        // Postconditions - The program is no longer running
        private void QuitDatabaseApp()
        {
            Environment.Exit(0);
        }

        // Function - Takes the user input and enters it
        // Outputs - The entered key is returned so that other methods can execute tasks based on entered value
        private ConsoleKeyInfo GetSelectionFromUser()
        {
            return Console.ReadKey();
        }

        // Function - display a menu for the user to select from
        // Postconditions - The main menu is printed so that the user can choose what to do next
        private void DisplayMainMenu()
        {
            Console.Write(@"
******************************
**Student Database Main Menu**
**[P]rint all records
**[C]reate a record
**[F]ind a record
**[U]pdate a record
**[D]elete a record
**[S]ave database w/o exit
**[Q]uit and save database
**
**ENTER SELECTION: ");
        }

        // Function - Takes the stored data and writes it to an output file
        // Postconditions - Whatever data was stored has been copied to the output file
        public void WriteDataToOutputFile()
        {
            //create the output file
            StreamWriter outfile = new StreamWriter("STUDENT_DATABASE_OUTPUT_FILE.txt");

            //output contents of list to the output file
            foreach (Student stu in students)
            {
                //output each student in the array list
                outfile.WriteLine(stu.ToStringFileFormat());
            }

            //close the output file
            outfile.Close();
        }

        // Function - Reads data one space at a time and writes it to the console
        // Postconditions - The data is printed to the user
        public void WriteDataToConsole()
        {
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine(students[i]);
            }
        }

        // Function - A test driver that makes student objects, and changes them 
        // Postconditions - Student objects are created, edited, and printed
        public void TestMain()
        {
            //make some students(POCO objects)
            Student stu01 = new Undergrad("Alice", "Anderson", "aanderson@uw.edu", YearRank.Freshman, 3.1f);
            Student stu02 = new Undergrad("Bob", "Bradshaw", "bbradshaw@uw.edu", YearRank.Sophomore, 3.2f);
            Student stu03 = new Undergrad("Chuck", "Costerella", "ccostarella@uw.edu", YearRank.Junior, 3.3f);

            Student stu04 = new GradStudent("Damn", "Daniel", "ddaniel@uw.edu", "Dr. Donald Chinn", 11111.99m);

            //but now with the new dynamic arraylist, here is the "add" operation
            students.Add(stu01);
            students.Add(stu02);
            students.Add(stu03);
            students.Add(stu04);


            //manipulate students in some way
            stu03.FirstName = "Chuck";
            stu03.LastName = "Costarella";
            stu03.EmailAddress = "costarec@uw.edu";


            //print out data for the students
            foreach (var stu in students)
            {
                //Console.WriteLine(stu);
            }
        }
    }
}
