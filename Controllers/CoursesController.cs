using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GraderApp.Models;

namespace GraderApp.Controllers
{
    public class CoursesController : Controller
    {
        private readonly GraderDBContext _context;

        public CoursesController(GraderDBContext context)
        {
            _context = context;
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? idCOURSE)
        {
            ViewBag.username = RouteData.Values["id"];
            if (idCOURSE == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(idCOURSE);
            if (course == null)
            {
                return NotFound();
            }

            ViewData["ProfessorsAfm"] = new SelectList(_context.Professors.Where( x => x.Afm != 0), "Afm", "Afm", course.ProfessorsAfm);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idCOURSE, [Bind("IdCourse,CourseTitle,CourseSemester,ProfessorsAfm")] Course course)
        {
            if (idCOURSE != course.IdCourse)
            {
                return NotFound();
            }

            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.IdCourse))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            ViewData["ProfessorsAfm"] = new SelectList(_context.Professors, "Afm", "Afm", course.ProfessorsAfm);
            ViewBag.username = RouteData.Values["id"];
            return RedirectToAction("assigncourses", "secretaries", new { id = ViewBag.username });
        }

        private bool CourseExists(int id)
        {
          return _context.Courses.Any(e => e.IdCourse == id);
        }
    }
}
