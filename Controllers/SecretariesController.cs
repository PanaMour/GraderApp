using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GraderApp.Models;
using X.PagedList;

namespace GraderApp.Controllers
{
    public class SecretariesController : Controller
    {
        private readonly GraderDBContext _context;

        public SecretariesController(GraderDBContext context)
        {
            _context = context;
        }

        // GET: Secretaries
        public async Task<IActionResult> Index()
        {
            var graderDBContext = _context.Secretaries.Include(s => s.UsersUsernameNavigation);
            ViewBag.username = RouteData.Values["id"];
            return View(await graderDBContext.ToListAsync());
        }
        //Function so that Secretaries can Insert Professors into the Database
        public IActionResult InsertProfessors()
        {
            ViewBag.username = RouteData.Values["id"];
            ViewBag.success = "";
            return View();
        }
        //Inserts Professors using ProfessorUser Model by first creating a User object
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertProfessors([Bind("Username,Password,AFM,Name,Surname,Department")] ProfessorUser professorUser)
        {
            ViewBag.username = RouteData.Values["id"];

            try
            {
                User user = new User() { Username = professorUser.Username, Password = professorUser.Password, Role = "Professor" };
                _context.Add(user);
                Professor professor = new Professor() { Afm = professorUser.AFM, Name = professorUser.Name, Surname = professorUser.Surname, Department = professorUser.Department, UsersUsername = professorUser.Username };
                _context.Add(professor);
                await _context.SaveChangesAsync();
            }
            catch {
                ViewBag.success = "An Error has occurred!";
                return View();
            }
            
            ViewBag.success = "Professor created successfully!";
            return View();
        }
        //Function so that Secretaries can Insert Students into the Database
        public IActionResult InsertStudents()
        {
            ViewBag.username = RouteData.Values["id"];
            ViewBag.success = "";
            return View();
        }
        //Inserts Students using StudentUser Model by first creating a User object
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertStudents([Bind("Username,Password,RegistrationNumber,Name,Surname,Department")] StudentUser studentUser)
        {
            ViewBag.username = RouteData.Values["id"];
            try
            {
                User user = new User() { Username = studentUser.Username, Password = studentUser.Password, Role = "Student" };
                _context.Add(user);
                Student student = new Student() { RegistrationNumber = studentUser.RegistrationNumber, Name = studentUser.Name, Surname = studentUser.Surname, Department = studentUser.Department, UsersUsername = studentUser.Username };
                _context.Add(student);
                await _context.SaveChangesAsync();
            }
            catch
            {
                ViewBag.success = "An Error has occurred!";
                return View();
            }
            ViewBag.success = "Student created successfully!";
            return View();
        }
        //Function so that Secretaries can Insert Courses into the Database
        public IActionResult InsertCourses()
        {
            ViewBag.username = RouteData.Values["id"];
            ViewBag.success = "";
            ViewData["sem"] = new SelectList("First", "Second", "Third", "Fourth");
            return View();
        }
        //Inserts Courses using the Course Model
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertCourses([Bind("IdCourse,CourseTitle,CourseSemester")] Course course)
        {
            ViewBag.username = RouteData.Values["id"];
            try
            {
                Course course1 = new Course() { IdCourse = course.IdCourse, CourseTitle = course.CourseTitle, CourseSemester = course.CourseSemester, ProfessorsAfm = 0 };
                _context.Add(course1);
                await _context.SaveChangesAsync();
            }
            catch
            {
                ViewBag.success = "An Error has occurred!";
                return View();
            }
            ViewBag.success = "Course created successfully!";
            return View();
        }
        //Displays all available courses to Secretaries
        public async Task<IActionResult> ViewCourses(int? page, string? search)
        {
            ViewBag.username = RouteData.Values["id"];

            var professors = (from c in _context.Professors select new { c.Afm, c.Name, c.Surname, c.Department }).ToList();
            var courses = (from c in _context.Courses select new { c.CourseTitle, c.CourseSemester, c.ProfessorsAfm }).ToList();
            List<SecretaryCourseView> secretaryCourse = new List<SecretaryCourseView>();

            foreach(var item in professors)
            {
                foreach(var item2 in courses)
                {
                    if (item.Afm == item2.ProfessorsAfm && item2.ProfessorsAfm != 0)
                    {
                        secretaryCourse.Add(new SecretaryCourseView(item2.CourseTitle,item2.CourseSemester,item.Name,item.Surname,item.Department, item2.ProfessorsAfm));
                    }
                }
            }

            ViewData["CurrentFilter"] = search;

            if (!String.IsNullOrEmpty(search))
            {
                secretaryCourse = secretaryCourse.Where(c => c.CourseTitle.ToLower().Contains(search.ToLower()) || c.Name.ToLower().Contains(search.ToLower()) || c.Surname.ToLower().Contains(search.ToLower())).ToList();
            }

            secretaryCourse = secretaryCourse.OrderBy(c => c.CourseTitle).ThenBy(c => c.Name).ThenBy(c => c.Surname).ToList();

            if (page != null && page < 1)
            {
                page = 1;
            }
            int PageSize = 5;
            var secretariesData = secretaryCourse.ToPagedList(page ?? 1, PageSize);

            return View(secretariesData);
        }
        //Allows Secretaries to assign a Professor's AFM to a Course that previously used dummy AFM=0
        public async Task<IActionResult> AssignCourses(int? page, String? search)
        {
            ViewBag.username = RouteData.Values["id"];

            ViewData["CurrentFilter"] = search;
            var courses = from c in _context.Courses
                            select c;
            List<Course> course = new List<Course>();
            foreach (var item in courses)
            {
                if(item.ProfessorsAfm == 0)
                {
                    course.Add(new Course() { IdCourse = item.IdCourse, CourseTitle = item.CourseTitle, CourseSemester = item.CourseSemester, ProfessorsAfm = item.ProfessorsAfm});
                }
            }
            if (!String.IsNullOrEmpty(search))
            {
                course = course.Where(c => c.CourseTitle.ToLower().Contains(search.ToLower())).ToList();
            }

            course = course.OrderBy(c => c.CourseTitle).ToList();

            if (page != null && page < 1)
            {
                page = 1;
            }
            int PageSize = 5;
            var courseData = course.ToPagedList(page ?? 1, PageSize);

            return View(courseData);
        }
        //Logout Function
        public IActionResult Logout()
        {
            return RedirectToAction(nameof(Index), "home");
        }
    }
}
