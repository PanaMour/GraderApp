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

        // GET: Secretaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Secretaries == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Phonenumber == id);
            if (secretary == null)
            {
                return NotFound();
            }

            return View(secretary);
        }

        // GET: Secretaries/Create
        public IActionResult Create()
        {
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username");
            return View();
        }

        // POST: Secretaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Phonenumber,Name,Surname,Department,UsersUsername")] Secretary secretary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(secretary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", secretary.UsersUsername);
            return View(secretary);
        }

        // GET: Secretaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Secretaries == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries.FindAsync(id);
            if (secretary == null)
            {
                return NotFound();
            }
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", secretary.UsersUsername);
            return View(secretary);
        }

        // POST: Secretaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Phonenumber,Name,Surname,Department,UsersUsername")] Secretary secretary)
        {
            if (id != secretary.Phonenumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(secretary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecretaryExists(secretary.Phonenumber))
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
            ViewData["UsersUsername"] = new SelectList(_context.Users, "Username", "Username", secretary.UsersUsername);
            return View(secretary);
        }

        // GET: Secretaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Secretaries == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries
                .Include(s => s.UsersUsernameNavigation)
                .FirstOrDefaultAsync(m => m.Phonenumber == id);
            if (secretary == null)
            {
                return NotFound();
            }

            return View(secretary);
        }

        // POST: Secretaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Secretaries == null)
            {
                return Problem("Entity set 'GraderDBContext.Secretaries'  is null.");
            }
            var secretary = await _context.Secretaries.FindAsync(id);
            if (secretary != null)
            {
                _context.Secretaries.Remove(secretary);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SecretaryExists(int id)
        {
          return _context.Secretaries.Any(e => e.Phonenumber == id);
        }

        public IActionResult InsertProfessors()
        {
            ViewBag.username = RouteData.Values["id"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertProfessors([Bind("Username,Password,AFM,Name,Surname,Department")] ProfessorUser professorUser)
        {
            ViewBag.username = RouteData.Values["id"];

            User user = new User() { Username = professorUser.Username, Password = professorUser.Password, Role = "Professor"};
            _context.Add(user);
            await _context.SaveChangesAsync();
            Professor professor = new Professor() { Afm = professorUser.AFM ,Name= professorUser.Name,Surname = professorUser.Surname,Department = professorUser.Department, UsersUsername = professorUser.Username};
            _context.Add(professor);
            await _context.SaveChangesAsync();
            return RedirectToAction("insertprofessors","secretaries", new { id = ViewBag.username ,success = "Professor created successfully!" });
        }

        public IActionResult InsertStudents()
        {
            ViewBag.username = RouteData.Values["id"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertStudents([Bind("Username,Password,RegistrationNumber,Name,Surname,Department")] StudentUser studentUser)
        {
            ViewBag.username = RouteData.Values["id"];

            User user = new User() { Username = studentUser.Username, Password = studentUser.Password, Role = "Student" };
            _context.Add(user);
            await _context.SaveChangesAsync();
            Student student = new Student() { RegistrationNumber = studentUser.RegistrationNumber, Name = studentUser.Name, Surname = studentUser.Surname, Department = studentUser.Department, UsersUsername = studentUser.Username };
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("insertstudents", "secretaries", new { id = ViewBag.username , success = "Student created successfully!"});
        }

        public IActionResult InsertCourses()
        {
            ViewBag.username = RouteData.Values["id"];
            ViewData["sem"] = new SelectList("First", "Second", "Third", "Fourth");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertCourses([Bind("IdCourse,CourseTitle,CourseSemester")] Course course)
        {
            ViewBag.username = RouteData.Values["id"];

            Course course1 = new Course() { IdCourse = course.IdCourse, CourseTitle = course.CourseTitle, CourseSemester = course.CourseSemester, ProfessorsAfm = 0 };
            _context.Add(course1);
            await _context.SaveChangesAsync();
            return RedirectToAction("insertcourses", "secretaries", new { id = ViewBag.username, success = "Course created successfully!" });
        }

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
        public IActionResult Logout()
        {
            return RedirectToAction(nameof(Index), "home");
        }
    }
}
