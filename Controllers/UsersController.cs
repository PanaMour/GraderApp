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
    public class UsersController : Controller
    {
        private readonly GraderDBContext _context;

        public UsersController(GraderDBContext context)
        {
            _context = context;
        }
        //Checks user's role and redirects him to according Index page
        public async Task<IActionResult> UserCheck(string username, string password)
        {
            if (username == null || password == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            if (password != user.Password)
            {
                return NotFound();
            }
            String role ="";
            switch (user.Role) {
                case "Student":
                    role = "students";
                    break;
                case "Professor":
                    role = "professors";
                    break;
                case "Secretary":
                    role = "secretaries";
                    break;
                    }

            return RedirectToAction(nameof(Index),role, new { id = username});
        }
    }
}
