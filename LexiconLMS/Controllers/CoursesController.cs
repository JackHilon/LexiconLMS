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
                    if (document.AppUser == student)
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

        private async Task<string> GetUserRole(ApplicationUser user)
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
                                                               ModuleId = m.Id,
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


            var course = await _context.Courses.FindAsync(id);

            // Remove all the students attending the course.

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
                        // get all the documents for activities and delete.
                        var getAllDocuments = _context.Documents.Include(a => a.ModuleActivity);

                        foreach (var activityitem in getActivitiesForModule)
                        {
                            var getDocumentForActivity = getAllDocuments.Where(m => m.ModuleActivityId == activityitem.Id);

                            foreach (var document in getDocumentForActivity)
                            {
                                _context.Documents.Remove(document);
                            }

                        }

                        // Delete all the activites for the module.

                        foreach (var activityitem in getActivitiesForModule)
                        {
                            _context.ModuleActivity.Remove(activityitem);
                        }
                    }
                }
            }

            // Remove all Module documents.

            var getAllModuleDocuments = _context.Documents.Include(a => a.Module);

            foreach (var document in getAllModulesForCourse)
            {
                var getDocumentForModule = getAllModuleDocuments.Where(m => m.ModuleId == document.Id);
                foreach (var doc in getDocumentForModule)
                {
                    _context.Documents.Remove(doc);
                }

            }

            // Remove all the modules of the course


            if (getAllModulesForCourse != null)
            {
                foreach (var module in getAllModulesForCourse)
                {
                    _context.Module.Remove(module);
                }
            }

            // Remove  all the documents of the course

            var getAllCourseDocuments = _context.Documents.Include(a => a.Course);

            foreach (var document in _context.Courses)
            {
                var getDocumentForcourse = getAllCourseDocuments.Where(m => m.CourseId == document.CourseId);
                foreach (var doc in getDocumentForcourse)
                {
                    _context.Documents.Remove(doc);
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

        public async Task<IActionResult> EditUser(string? id,string UserType)
        {
            ViewBag.UserType = UserType;
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
        public async Task<IActionResult> EditUser(string id, [Bind("Id, SecondUserName, UserName, Email, CourseId")] ApplicationUser user)
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
        public async Task<IActionResult> DeleteUser(string? id ,string UserType)
        {
            ViewBag.UserType = UserType;
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser user = await GetAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            string userrole = await GetUserRole(user);
            if(userrole == "Teacher")
            {
                ViewBag.Role = "Teacher";
            }
            else
            {
                ViewBag.Role = "Student";
            }
            
            

            return View(user);
        }

        private async Task<ApplicationUser> GetAsync(string? id)
        {
            return await _context.AppUser
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
        }


        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {


            /*var user = await GetAsync(id);

            if (user.Documents != null)
            {
                foreach (var document in user.Documents)
                {
                    user.Documents.Remove(document);
                    _context.Documents.Remove(document);
                }
            }

            string userRole = await GetUserRole(user);
            int courseID = (int) user.CourseId;
            string courseName = user.Course.CourseName;


            
            _context.AppUser.Remove(user);
            await _context.SaveChangesAsync();

            if (userRole == "Teacher")
            {
                return RedirectToAction(nameof(ListOfTeachers));
            }
            else
            {
                return RedirectToAction(nameof(ListOfCourseStudents), courseName, courseID);
            }*/


            var user = await GetAsync(id);

            // ----------------------------------------------
            DeleteRelatedUserDoc(id);
            // ----------------------------------------------
            _context.AppUser.Remove(user);
            await _context.SaveChangesAsync();

            var userRole = await GetUserRole(user);
            if (userRole == "Teacher")
            {
                return RedirectToAction(nameof(ListOfTeachers));
            }
            else
            {
                var course = _context.AppUser.FirstOrDefault(u => u.Id == id).Course;
                var courseName = course.CourseName;
                var courseID = course.CourseId;
                // ===================================================================
                //ViewBag.nameCourse = CourseName;
                ViewBag.IdCourse = id;

                var allStudents = await userManager.GetUsersInRoleAsync("Student");
                var students = allStudents.Where(s => s.CourseId == courseID);

                var documents = await _context.Documents.ToListAsync();
                foreach (var student in students)
                {
                    foreach (var document in documents)
                    {
                        if (document.AppUser == student)
                        {
                            student.Documents.Add(document);
                        }
                    }
                }
                // =====================================================================

                return View("ListOfCourseStudents", students);
            }// end of else
        }
        private void DeleteRelatedUserDoc(string id)
        {
            var docs = _context.Documents.Where(a => a.AppUser.Id == id).ToList();
            foreach (var doc in docs)
            {
                var docId = doc.DocumentId;
                _context.Documents.Remove(doc);
            }
        }


        // ================ end of Edit a teacher ============

        //private async Task<string> GetUserRrole(ApplicationUser user)
        //{
        //    string userRole;

        //    bool flag = await userManager.IsInRoleAsync(user, "Teacher");

        //    if (flag)
        //        userRole = "Teacher";
        //    else
        //        userRole = "Student";

        //    return userRole;
        //}

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
