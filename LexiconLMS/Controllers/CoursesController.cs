using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LexiconLMS.Data;
using LexiconLMS.Models;
using Microsoft.AspNetCore.Identity;
using LexiconLMS.Areas.Identity.Pages.Account;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace LexiconLMS.Controllers
{

    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public CoursesController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> ListOfTeachers()             //  <-------------------- List of users (teachers) -------
        {
             var AllTeachers = await userManager.GetUsersInRoleAsync("Teacher");
            
            return  View(AllTeachers);
        }

        [HttpGet]
        public async Task<IActionResult> ListOfCourseStudents(int? id)             //  <-------------------- List of Course's students -------
        {
        

            var allStudents = await userManager.GetUsersInRoleAsync("Student");
            var students = allStudents.Where(s => s.CourseId == id);
            return View(students);
        }

        private async Task<string> GetUserRrole(ApplicationUser user)
        {
            string userRole;

            bool flag = await userManager.IsInRoleAsync(user, "Teacher");

            if (flag)
                userRole = "Teacher";
            else
                userRole = "Student";

            return userRole;
        }

        // ----------------------------------------------------------------------------------------------------------------------



        // GET: Courses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Courses.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var course = await _context.Courses
            //    .FirstOrDefaultAsync(m => m.CourseId == id);
            //if (course == null)
            //{
            //    return NotFound();
            //}

            //  return View(course);

            TempData["Courseid"] = id;
        //    TempData["CreateCourseid"] = id;
         
            return RedirectToAction("ModuleActivity", "Modules");
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,CourseName,CourseDescription,StartDate,AppUsers")] Course course)
        {
            if (ModelState.IsValid)
            {
                course.StartDate = DateTime.Now;
                
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,CourseName,CourseDescription")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newUpdatedDataCourse = await _context.Courses.FindAsync(id);

                    if(newUpdatedDataCourse == null)
                    {
                        return NotFound();
                    }

                    newUpdatedDataCourse.CourseId = course.CourseId;
                    newUpdatedDataCourse.CourseName = course.CourseName;
                    newUpdatedDataCourse.CourseDescription = course.CourseDescription;
                  //  _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
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
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }


        // ----------------- Edit a teacher -------------------------------------------------------

        public async Task<IActionResult> EditUser(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.AppUser.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, [Bind("Id, Name, UserName, Email")] ApplicationUser user)
        {
             if(!string.Equals(id, user.Id))                       //if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newUpdatedAppUser = await _context.AppUser.FindAsync(id);

                    if (newUpdatedAppUser == null)
                    {
                        return NotFound();
                    }

                    newUpdatedAppUser.Name = user.Name;
                    newUpdatedAppUser.Email = user.Email;
                    newUpdatedAppUser.UserName = user.UserName;
                    newUpdatedAppUser.NormalizedUserName = user.NormalizedUserName;
                    newUpdatedAppUser.NormalizedEmail = user.NormalizedEmail;
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListOfTeachers));
            }
            return View(user);
        }

        private bool AppUserExists(string id)
        {
            return GetAny(id);
        }

        private bool GetAny(string id)
        {
            return _context.AppUser.Any(e => e.Id == id);
        }

        // -------------- end of Edit a teacher -----------------------------------------------------------------------



        // =============== Delete a teacher ================
        public async Task<IActionResult> DeleteUser(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser user = await GetAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        private async Task<ApplicationUser> GetAsync(string? id)
        {
            return await _context.AppUser
                .FirstOrDefaultAsync(m => m.Id == id);
        }


        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await GetAsync(id);

            _context.AppUser.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListOfTeachers));
        }

        // ================ end of Edit a teacher ============

    }
}
