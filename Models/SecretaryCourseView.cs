namespace GraderApp.Models
{
    public class SecretaryCourseView
    {
        public SecretaryCourseView(string courseTitle, string courseSemester, string name, string surname, string department, int aFM)
        {
            CourseTitle = courseTitle;
            CourseSemester = courseSemester;
            Name = name;
            Surname = surname;
            Department = department;
            AFM = aFM;
        }

        public string CourseTitle { get; set; }
        public string CourseSemester { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Department { get; set; }
        public int AFM { get; set; }
    }
}
