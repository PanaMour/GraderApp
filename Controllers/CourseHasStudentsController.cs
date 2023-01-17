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

        // GET: CourseHasStudents
        public async Task<IActionResult> Index()
        {
            var graderDBContext = _context.CourseHasStudents.Include(c => c.CourseIdCourseNavigation).Include(c => c.StudentsRegistrationNumberNavigation);
            return View(await graderDBContext.ToListAsync());
        }

        // GET: CourseHasStudents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CourseHasStudents == null)
            {
                return NotFound();
            }

            var courseHasStudent = await _context.CourseHasStudents
                .Include(c => c.CourseIdCourseNavigation)
                .Include(c => c.StudentsRegistrationNumberNavigation)
                .FirstOrDefaultAsync(m => m.CourseIdCourse == id);
            if (courseHasStudent == null)
            {
                return NotFound();
            }

            return View(courseHasStudent);
        }

        // GET: CourseHasStudents/Create
        public IActionResult Create()
        {
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
            if (ModelState.IsValid)
            {
                _context.Add(courseHasStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseIdCourse"] = new SelectList(_context.Courses, "IdCourse", "IdCourse", courseHasStudent.CourseIdCourse);
            ViewData["StudentsRegistrationNumber"] = new SelectList(_context.Students, "RegistrationNumber", "RegistrationNumber", courseHasStudent.StudentsRegistrationNumber);
            return View(courseHasStudent);
        }

        // GET: CourseHasStudents/Edit/5
        public IActionResult Edit(int? COURSE_idCOURSE, int? STUDENTS_RegistrationNumber)
        {
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

        // GET: CourseHasStudents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CourseHasStudents == null)
            {
                return NotFound();
            }

            var courseHasStudent = await _context.CourseHasStudents
                .Include(c => c.CourseIdCourseNavigation)
                .Include(c => c.StudentsRegistrationNumberNavigation)
                .FirstOrDefaultAsync(m => m.CourseIdCourse == id);
            if (courseHasStudent == null)
            {
                return NotFound();
            }

            return View(courseHasStudent);
        }

        // POST: CourseHasStudents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CourseHasStudents == null)
            {
                return Problem("Entity set 'GraderDBContext.CourseHasStudents'  is null.");
            }
            var courseHasStudent = await _context.CourseHasStudents.FindAsync(id);
            if (courseHasStudent != null)
            {
                _context.CourseHasStudents.Remove(courseHasStudent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseHasStudentExists(int id)
        {
          return _context.CourseHasStudents.Any(e => e.CourseIdCourse == id);
        }
    }
}
