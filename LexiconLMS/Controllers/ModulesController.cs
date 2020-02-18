using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LexiconLMS.Data;
using LexiconLMS.Models;
using LexiconLMS.ViewModels;

namespace LexiconLMS.Controllers
{
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {


            var applicationDbContext = _context.Module.Include(q => q.Course);
            return View(await applicationDbContext.ToListAsync());
        }

     
        // View activity screen as partial view in the same page as Modules.
        public ActionResult ActivityPartialView(int? id ,string ModulsName)
        {
            ModulsName = _context.Module.Find(id).Name; // <-- added by jack
            ViewBag.ModulsName = ModulsName;
            var activityView = _context.ModuleActivity
                .Include(m => m.Module).Include(d => d.Documents);
            var _thisActivityView = activityView.Where(a => a.ModuleId == id);
     
            return PartialView("ActivityPartialView", _thisActivityView);
        }
        public ActionResult ModulePartialView()
        {
            // Display Modules for the corresponding course.
            int data = (int)TempData["Courseid"];
            TempData.Keep();
            var applicationDbContext = _context.Module.Include(q => q.Course);
            var _thisModuleCourse =  applicationDbContext.Where(c => c.CourseId == data).Include(a => a.Activity).Include(a => a.Documents);
            return View(_thisModuleCourse);

        }
            // GET: Modules/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            } 

            var @module = await _context.Module
                .Include(a=> a.Course)
                .Include(a => a.Documents)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Modules/Create
        public IActionResult Create()
        {
            ViewBag.CourseName = TempData["CourseName"];
            TempData.Keep();
          //  ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartDate,CourseId")] Module @module)
        {
             var CourseId = (int)TempData["Courseid"];
            TempData.Keep();
            
            //******* Get Course start date  ******
            var couresStartDate = _context.Courses
            .Where(c => c.CourseId == CourseId).Select(s => s.StartDate).FirstOrDefault();
            //******* Check Module start date after Course start ******
            if (@module.StartDate < couresStartDate)
            {
                ModelState.AddModelError("StartDate", $"The Module's start date must be after {couresStartDate}");

            }
            if (ModelState.IsValid)
            {
                // Supply the course name when you save the module record.

                int data = (int)TempData["Courseid"];
                TempData.Keep();
                module.CourseId = data;
                _context.Add(@module);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ModulePartialView));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", @module.CourseId);
            
           return View(@module);
        }

        // GET: Modules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Module.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName", @module.CourseId);
           
            return View(@module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,CourseId")] Module @module)
        {
            if (id != @module.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ModulePartialView));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseId", @module.CourseId);
            return View(@module);
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Module
                .Include(q => q.Course).Include(a => a.Activity)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
         // Remove all activities related to the module.

            var getAllActivities = _context.ModuleActivity.Include(m => m.Module);

            var getActivitiesForModule = getAllActivities.Where(m => m.ModuleId == id);
         
            //get all the documents.
            var getAllDocuments = _context.Documents.Include(a => a.ModuleActivity);

            // Delete all the documents for the activities of the module
            foreach (var activityitem in getActivitiesForModule)
            {
                var getDocumentForActivity = getAllDocuments.Where(m => m.ModuleActivityId == activityitem.Id);

                foreach (var document in getDocumentForActivity)
                {
                    _context.Documents.Remove(document);
                }

              

            }

            // Remove all the activities
            foreach (var activityitem in getActivitiesForModule)
            {
                _context.ModuleActivity.Remove(activityitem);

            }

            // Remove all the documents for the module

            var getAllModuleDocuments = _context.Documents.Include(a => a.Module);
            var getDocumentForModule = getAllModuleDocuments.Where(m => m.ModuleId == id);

            foreach (var document in getDocumentForModule)
            {
                _context.Documents.Remove(document);
            }

            // remove the module
            var @module = await _context.Module.FindAsync(id);
            _context.Module.Remove(@module);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ModulePartialView));
        }

        private bool ModuleExists(int id)
        {
            return _context.Module.Any(e => e.Id == id);
        }

    }
}
