using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GraderApp.Models;
using System.Data;
using X.PagedList;
using Azure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GraderApp.Controllers
{
    public class StudentsController : Controller
    {
        private readonly GraderDBContext _context;

        public StudentsController(GraderDBContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var graderDBContext = _context.Students.Include(s => s.UsersUsernameNavigation);
            ViewBag.username = RouteData.Values["id"];
            return View(await graderDBContext.ToListAsync());
        }
        //Function that displays all Courses student is registered in 
        public async Task<IActionResult> GradesPerCourse(int? page, string? search)
        {
            ViewBag.username = RouteData.Values["id"];

            int regNum = 0;
            var students = (from c in _context.Students select new { c.RegistrationNumber, c.UsersUsername }).ToList();
            var courses = (from c in _context.Courses select new { c.IdCourse, c.CourseTitle, c.CourseSemester }).ToList();
            var courseHasStudents = (from c in _context.CourseHasStudents select new {c.CourseIdCourse, c.StudentsRegistrationNumber, c.GradeCourseStudent });
            List<StudentCourseView> studentCourse = new List<StudentCourseView>();
            foreach (var item in students)
            {
                if (ViewBag.username == item.UsersUsername)
                    regNum = item.RegistrationNumber;
            }
            foreach (var item in courseHasStudents)
                if (regNum == item.StudentsRegistrationNumber)
                {
                    foreach(var item2 in courses)
                    {
                        if (item.CourseIdCourse == item2.IdCourse)
                        {
                            studentCourse.Add(new StudentCourseView (item.CourseIdCourse, item2.CourseTitle,item2.CourseSemester,item.GradeCourseStudent));
                        }
                    }
                }

            ViewData["CurrentFilter"] = search;

            if (!String.IsNullOrEmpty(search))
            {
                studentCourse = studentCourse.Where(c => c.CourseTitle.ToLower().Contains(search.ToLower())).ToList();
            }

            studentCourse = studentCourse.OrderBy(c => c.CourseTitle).ToList();

            if (page != null && page < 1)
            {
                page = 1;
            }
            int PageSize = 5;
            var studentsData = studentCourse.ToPagedList(page ?? 1, PageSize);

            return View(studentsData);
        }
        //Function that displays all Courses student is registered in based on Semester
        public async Task<IActionResult> GradesPerSemester(int? page, string? search)
        {
            ViewBag.username = RouteData.Values["id"];

            int regNum = 0;
            var students = (from c in _context.Students select new { c.RegistrationNumber, c.UsersUsername }).ToList();
            var courses = (from c in _context.Courses orderby c.CourseSemester select new { c.IdCourse, c.CourseTitle, c.CourseSemester }).ToList();
            var courseHasStudents = (from c in _context.CourseHasStudents select new { c.CourseIdCourse, c.StudentsRegistrationNumber, c.GradeCourseStudent });
            List<StudentCourseView> studentCourse = new List<StudentCourseView>();
            foreach (var item in students)
            {
                if (ViewBag.username == item.UsersUsername)
                    regNum = item.RegistrationNumber;
            }
            foreach (var item in courseHasStudents)
                if (regNum == item.StudentsRegistrationNumber)
                {
                    foreach (var item2 in courses)
                    {
                        if (item.CourseIdCourse == item2.IdCourse)
                        {
                            studentCourse.Add(new StudentCourseView(item.CourseIdCourse, item2.CourseTitle, item2.CourseSemester, item.GradeCourseStudent));
                        }
                    }
                }

            ViewData["CurrentFilter"] = search;

            if (!String.IsNullOrEmpty(search))
            {
                studentCourse = studentCourse.Where(c => c.CourseTitle.ToLower().Contains(search.ToLower())).ToList();
            }

            studentCourse = studentCourse.OrderBy(c => c.CourseTitle).ToList();
            if (page != null && page < 1)
            {
                page = 1;
            }
            if (page == 2)
                ViewBag.courseSem = "Second";
            else if (page ==3)
                ViewBag.courseSem = "Third";
            else if (page == 4)
                ViewBag.courseSem = "Fourth";
            else ViewBag.courseSem = "First";
            var studentsData = studentCourse.ToPagedList();            
            ViewBag.search = search;
            return View(studentsData);
        }
        //Function that displays all Courses that student has a Grade in
        public async Task<IActionResult> TotalGrades(int? page, String? search)
        {
            ViewBag.username = RouteData.Values["id"];

            int regNum = 0;
            var students = (from c in _context.Students select new { c.RegistrationNumber, c.UsersUsername }).ToList();
            var courses = (from c in _context.Courses select new { c.IdCourse, c.CourseTitle, c.CourseSemester }).ToList();
            var courseHasStudents = (from c in _context.CourseHasStudents select new {c.CourseIdCourse, c.StudentsRegistrationNumber, c.GradeCourseStudent });
            List<StudentCourseView> studentCourse = new List<StudentCourseView>();
            foreach (var item in students)
            {
                if (ViewBag.username == item.UsersUsername)
                    regNum = item.RegistrationNumber;
            }

            float? count = 0;
            float? sum = 0;
            decimal avg = 0;
            foreach (var item in courseHasStudents)
                if (regNum == item.StudentsRegistrationNumber)
                {
                    foreach(var item2 in courses)
                    {
                        if (item.CourseIdCourse == item2.IdCourse && item.GradeCourseStudent != null)
                        {
                            studentCourse.Add(new StudentCourseView (item.CourseIdCourse, item2.CourseTitle,item2.CourseSemester,item.GradeCourseStudent));
                                sum += item.GradeCourseStudent;
                                count++;
                            }
                        }
                }
            ViewData["CurrentFilter"] = search;

            if (!String.IsNullOrEmpty(search))
            {
                studentCourse = studentCourse.Where(c => c.CourseTitle.ToLower().Contains(search.ToLower())).ToList();
            }

            studentCourse = studentCourse.OrderBy(c => c.CourseTitle).ToList();
            if (page != null && page < 1)
            {
                page = 1;
            }
            int PageSize = 10;
            var studentsData = studentCourse.ToPagedList(page ?? 1, PageSize);
            if (count > 0)
            {
                avg = (decimal)(sum / count);

                avg = Math.Round(avg, 2);
            }
            ViewBag.avg = avg;
            return View(studentsData);
        }
        //Logout Function
        public IActionResult Logout()
        {
            return RedirectToAction(nameof(Index), "home");
        }
    }
}
