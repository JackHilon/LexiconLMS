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
using LexiconLMS.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace LexiconLMS.Controllers
{
    [Authorize]
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

            return View(AllTeachers);
        }

        [HttpGet]
        public async Task<IActionResult> ListOfCourseStudents(int? id, string CourseName)             //  <-------------------- List of Course's students -------
        {

            ViewBag.nameCourse = CourseName;
            ViewBag.IdCourse = id;

            var allStudents = await userManager.GetUsersInRoleAsync("Student");
            var students = allStudents.Where(s => s.CourseId == id);

            var documents = await _context.Documents.ToListAsync();
            foreach (var student in students)
            {
                foreach (var document in documents)
                {
                    if(document.AppUser == student)
                    {
                        student.Documents.Add(document);
                    }
                }
            }

            return View(students);

            //*****************Jack's method*****************
            //string courseName = _context.Courses.FirstOrDefault(c => c.CourseId == id).CourseName;
            //var allStudents = await userManager.GetUsersInRoleAsync("Student");
            //var students = allStudents.Where(s => s.CourseId == id);

            //var model = new StudentListAndCourseName()
            //{
            //    CourseName = courseName,
            //    Students = students
            //};

            //return View(model);
            //********************************
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
            if (User.IsInRole("Teacher"))
            {
                return View(await _context.Courses.Include(a => a.Documents).ToListAsync());
            }
            else if (User.IsInRole("Student"))
            {
                //Populera StudentsModelViewModel och CoursesModulesForStudentsViewModel
                var CourseAppUser = await userManager.GetUserAsync(User);
                var CourseId = CourseAppUser.CourseId;
                var Course = _context.Courses.Find(CourseId);
                var CourseName = Course.CourseName;
                ViewBag.CourseName = CourseName;
                ViewBag.CourseId = CourseId;

                List<int> modulesIds = new List<int>();
                List<Module> Modules = await _context.Module.Where(m => m.CourseId == CourseId).ToListAsync();
                foreach (var module in Modules)
                {
                    modulesIds.Add(module.Id);
                }

                List<StudentsModelViewModel> LotsOfModules = _context.Module
                                                           .Where(m => m.CourseId == CourseId)
                                                           .Include(m => m.Activity).ThenInclude(d => d.Documents).ThenInclude(a => a.AppUser)
                                                           .Select(m => new StudentsModelViewModel
                                                           {
                    ModuleName = m.Name,
                    ModuleDescription = m.Description,
                    ModuleStartDate = m.StartDate,
                    
                    //Activities = _context.ModuleActivity.Where(a => a.ModuleId == m.Id).Select(a => new StudentsActivityViewModel
                    //{
                    //    ActivityName = a.Name,
                    //    ActivityDescription = a.Description,
                    //    ActivityStartDate = a.StartDate
                    //}).ToList()
                    Activities = m.Activity
                }
                ).ToList();

                CoursesModulesForStudentsViewModel viewModel = _context.Module.Select(m => new CoursesModulesForStudentsViewModel
                {
                    ModuleName = m.Name,
                    ModuleStartDate = m.StartDate,
                    ModuleDescription = m.Description,
                    MyModules = LotsOfModules
                }
                ).First();
              
                return View(nameof(StudentListings), viewModel);
                //return RedirectToAction(nameof(StudentListings), viewModel);
            }

            return View();
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id, string CourseName)
        {
            // Pass the Course id to Modules.

            TempData["Courseid"] = id;
            TempData["CourseName"] = CourseName;
            //ViewData["CourseName"] = CourseName;
            // ViewData["CourseName11"] = CourseName;


            return RedirectToAction("ModulePartialView", "Modules");
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

                    if (newUpdatedDataCourse == null)
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
        public async Task<IActionResult> Delete(int? id, string CourseName)
        {
            // Get list of students attending the course.

            ViewBag.nameCourse = CourseName;
            ViewBag.IdCourse = id;

            var allStudents = await userManager.GetUsersInRoleAsync("Student");
            var students = allStudents.Where(s => s.CourseId == id);

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
        public async Task<IActionResult> DeleteConfirmed(int id, string CourseName)
        {
            // Remove all the students attending the course.

            var course = await _context.Courses.FindAsync(id);

            var allStudents = await userManager.GetUsersInRoleAsync("Student");
            var students = allStudents.Where(s => s.CourseId == id);
            if (students != null)
            {
                foreach (var student in students)
                {
                    _context.AppUser.Remove(student);
                }
            }
            //To  Remove all activities related to the module.
            // Get all the modules for the course.

            var getallModules = _context.Module.Include(c => c.Course);

            var getAllModulesForCourse = getallModules.Where(c => c.CourseId == id);

            if (getAllModulesForCourse != null)
            {
                foreach (var module in getAllModulesForCourse)
                {
                    var getAllActivities = _context.ModuleActivity.Include(m => m.Module);

                    var getActivitiesForModule = getAllActivities.Where(m => m.ModuleId == module.Id);

                    if (getActivitiesForModule != null)
                    {

                        foreach (var activityitem in getActivitiesForModule)
                        {
                            _context.ModuleActivity.Remove(activityitem);
                        }
                    }
                }
            }

            // Remove all the modules of the course

            // var allModules =  _context.Module.Include(c => c.Course);

            //var getAllModulesForCourse = allModules.Where(c => c.CourseId == id);
            if (getAllModulesForCourse != null)
            {
                foreach (var module in getAllModulesForCourse)
                {
                    _context.Module.Remove(module);
                }
            }
            // Remove course.

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
        public async Task<IActionResult> EditUser(string id, [Bind("Id, Name, UserName, Email, CourseId")] ApplicationUser user)
        {
            var hisCourseId = user.CourseId;

            if (!string.Equals(id, user.Id))                       //if (id != user.Id)            
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

                    newUpdatedAppUser.SecondUserName = user.SecondUserName;                                     // --> Sychronize the (Email, UserName, NormalizedUserName, NormalizedEmail) to Email
                    newUpdatedAppUser.Email = user.Email;                                   // -- REMARK! -- NormalizedEmail must be unique --
                    newUpdatedAppUser.UserName = user.Email;
                    newUpdatedAppUser.NormalizedUserName = user.Email.ToUpper();
                    newUpdatedAppUser.NormalizedEmail = newUpdatedAppUser.NormalizedUserName;

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
                if (await userManager.IsInRoleAsync(user, "Student"))
                    return RedirectToAction(nameof(ListOfCourseStudents), user.CourseId);
                else
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


        public IActionResult StudentListings()
        {
            return View(nameof(StudentListings));
        }

    }

    internal class ViewObject
    {
        public List<Module> MyList { get; set; }
        public ViewObject(List<Module> moduleList)
        {
            MyList = moduleList;
        }

    }
}
