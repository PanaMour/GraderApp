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
    public class ProfessorsController : Controller
    {
        private readonly GraderDBContext _context;

        public ProfessorsController(GraderDBContext context)
        {
            _context = context;
        }

        // GET: Professors
        public async Task<IActionResult> Index()
        {
            var graderDBContext = _context.Professors.Include(p => p.UsersUsernameNavigation);
            ViewBag.username = RouteData.Values["id"];
            return View(await graderDBContext.ToListAsync());
        }

        //Function so that Professor can View Graded Courses through ProfessorCourseView Model
        public async Task<IActionResult> ViewGrades(int? page, String? search)
        {
            ViewBag.username = RouteData.Values["id"];

            int profAFM = 0;
            var professors = (from c in _context.Professors select new { c.Afm, c.UsersUsername }).ToList();
            var students = (from c in _context.Students select new { c.RegistrationNumber }).ToList();
            var courses = (from c in _context.Courses select new { c.IdCourse, c.CourseTitle, c.CourseSemester, c.ProfessorsAfm }).ToList();
            var courseHasStudents = (from c in _context.CourseHasStudents select new { c.CourseIdCourse, c.StudentsRegistrationNumber, c.GradeCourseStudent });
            List<ProfessorCourseView> professorCourse = new List<ProfessorCourseView>();
            foreach(var item in professors)
            {
                if (ViewBag.username == item.UsersUsername)
                    profAFM = item.Afm;
            }
            foreach(var item in courses)
            {
                if(profAFM == item.ProfessorsAfm)
                {
                    foreach(var item2 in courseHasStudents)
                    {
                        if(item.IdCourse == item2.CourseIdCourse)
                        {
                            foreach(var item3 in students)
                            {
                                if(item2.StudentsRegistrationNumber == item3.RegistrationNumber)
                                {
                                    if(item2.GradeCourseStudent != null)
                                        professorCourse.Add(new ProfessorCourseView(item.IdCourse, item3.RegistrationNumber, item.CourseTitle, item.CourseSemester, item2.GradeCourseStudent));
                                }
                            }
                        }
                    }
                }
            }

            ViewData["CurrentFilter"] = search;

            if (!String.IsNullOrEmpty(search))
            {
                professorCourse = professorCourse.Where(c => c.CourseTitle.ToLower().Contains(search.ToLower())).ToList();
            }

            professorCourse = professorCourse.OrderBy(c => c.CourseTitle).ToList();

            if (page != null && page < 1)
            {
                page = 1;
            }
            int PageSize = 5;
            var professorsData = professorCourse.ToPagedList(page ?? 1, PageSize);

            return View(professorsData);
        }
        //Function so that Professor can Insert Grades in ungraded courses through CourseHasStudent Controller and Action Edit
        public async Task<IActionResult> InsertGrades(int? page, String? search)
        {
            ViewBag.username = RouteData.Values["id"];

            int profAFM = 0;
            var professors = (from c in _context.Professors select new { c.Afm, c.UsersUsername }).ToList();
            var students = (from c in _context.Students select new { c.RegistrationNumber }).ToList();
            var courses = (from c in _context.Courses select new { c.IdCourse, c.CourseTitle, c.CourseSemester, c.ProfessorsAfm }).ToList();
            var courseHasStudents = (from c in _context.CourseHasStudents select new { c.CourseIdCourse, c.StudentsRegistrationNumber, c.GradeCourseStudent });
            List<ProfessorCourseView> professorCourse = new List<ProfessorCourseView>();
            foreach (var item in professors)
            {
                if (ViewBag.username == item.UsersUsername)
                    profAFM = item.Afm;
            }
            foreach (var item in courses)
            {
                if (profAFM == item.ProfessorsAfm)
                {
                    foreach (var item2 in courseHasStudents)
                    {
                        if (item.IdCourse == item2.CourseIdCourse)
                        {
                            foreach (var item3 in students)
                            {
                                if (item2.StudentsRegistrationNumber == item3.RegistrationNumber)
                                {
                                    if (item2.GradeCourseStudent == null)
                                        professorCourse.Add(new ProfessorCourseView(item.IdCourse, item3.RegistrationNumber, item.CourseTitle, item.CourseSemester, item2.GradeCourseStudent));
                                }
                            }
                        }
                    }
                }
            }

            ViewData["CurrentFilter"] = search;

            if (!String.IsNullOrEmpty(search))
            {
                professorCourse = professorCourse.Where(c => c.CourseTitle.ToLower().Contains(search.ToLower())).ToList();
            }

            professorCourse = professorCourse.OrderBy(c => c.CourseTitle).ToList();

            if (page != null && page < 1)
            {
                page = 1;
            }
            int PageSize = 5;
            var professorsData = professorCourse.ToPagedList(page ?? 1, PageSize);

            return View(professorsData);
        }

        //Logout Function
        public IActionResult Logout()
        {
        return RedirectToAction(nameof(Index), "home");
        }
    }
}
