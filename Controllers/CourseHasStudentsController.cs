using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GraderApp.Models;
using System.Data;

namespace GraderApp.Controllers
{
    public class CourseHasStudentsController : Controller
    {
        private readonly GraderDBContext _context;

        public CourseHasStudentsController(GraderDBContext context)
        {
            _context = context;
        }

        // GET: CourseHasStudents/Create
        public IActionResult Create()
        {
            ViewBag.username = RouteData.Values["id"];
            ViewBag.success = "";
            ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "IdCourse");
            ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students, "RegistrationNumber", "RegistrationNumber");
            return View();
        }

        // POST: CourseHasStudents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseIdCourse,StudentsRegistrationNumber,GradeCourseStudent")] CourseHasStudent courseHasStudent)
        {
            ViewBag.username = RouteData.Values["id"];
            
            foreach (var item in _context.CourseHasStudents)
            {
                if(courseHasStudent.StudentsRegistrationNumber == item.StudentsRegistrationNumber && courseHasStudent.CourseIdCourse == item.CourseIdCourse)
                {
                    ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "IdCourse", courseHasStudent.CourseIdCourse);
                    ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students, "RegistrationNumber", "RegistrationNumber", courseHasStudent.StudentsRegistrationNumber);
                    ViewBag.success = "Student is already registered for that course!";
                    return View();
                }
            }
            ViewBag.success = "Registered Student to Course successfully!";
            _context.Add(courseHasStudent);
            await _context.SaveChangesAsync();
            ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "IdCourse", courseHasStudent.CourseIdCourse);
            ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students, "RegistrationNumber", "RegistrationNumber", courseHasStudent.StudentsRegistrationNumber);
            
            return View();
        }

        // GET: CourseHasStudents/Edit/5
        public IActionResult Edit(int? COURSE_idCOURSE, int? STUDENTS_RegistrationNumber)
        {
            ViewBag.username = RouteData.Values["id"];
            if (COURSE_idCOURSE == null || STUDENTS_RegistrationNumber == null || _context.CourseHasStudents == null)
            {
                return NotFound();     
            }

            CourseHasStudent courseHasStudent = _context.CourseHasStudents.Find(COURSE_idCOURSE, STUDENTS_RegistrationNumber);
            if (courseHasStudent == null)
            {
                return NotFound();
            }
            ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "IdCourse", courseHasStudent.CourseIdCourse);
            ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students, "RegistrationNumber", "RegistrationNumber", courseHasStudent.StudentsRegistrationNumber);
            return View(courseHasStudent);
        }

        // POST: CourseHasStudents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int CourseIdCourse, int StudentsRegistrationNumber, [Bind("CourseIdCourse,StudentsRegistrationNumber,GradeCourseStudent")] CourseHasStudent courseHasStudent)
        {
            if (CourseIdCourse != courseHasStudent.CourseIdCourse || StudentsRegistrationNumber != courseHasStudent.StudentsRegistrationNumber)
            {
                return NotFound();
            }

            try
            {
                _context.Update(courseHasStudent);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseHasStudentExists(courseHasStudent.CourseIdCourse))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewBag.username = RouteData.Values["id"];
            return RedirectToAction("insertgrades", "professors", new { id = ViewBag.username });
        }

        private bool CourseHasStudentExists(int id)
        {
          return _context.CourseHasStudents.Any(e => e.CourseIdCourse == id);
        }
    }
}
