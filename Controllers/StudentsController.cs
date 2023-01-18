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

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.RegistrationNumber == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegistrationNumber,Name,Surname,Department,UsersUsername")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", student.UsersUsername);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", student.UsersUsername);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegistrationNumber,Name,Surname,Department,UsersUsername")] Student student)
        {
            if (id != student.RegistrationNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.RegistrationNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", student.UsersUsername);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.RegistrationNumber == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'GraderDBContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
          return _context.Students.Any(e => e.RegistrationNumber == id);
        }

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

        public IActionResult Logout()
        {
            return RedirectToAction(nameof(Index), "home");
        }
    }
}
