namespace GraderApp.Models
{
    public class StudentCourseView
    {
        public StudentCourseView(int? idCourse, int? courseRegistrationNumber, string? courseTitle, string? courseSemester, int? courseGradeStudent)
        {
            IdCourse = idCourse;
            CourseRegistrationNumber = courseRegistrationNumber;
            CourseTitle = courseTitle;
            CourseSemester = courseSemester;
            CourseGradeStudent = courseGradeStudent;
        }

        public int? IdCourse { get; set; }
        public int? CourseRegistrationNumber { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseSemester { get; set; }
        public int? CourseGradeStudent { get; set; }

    }
}
