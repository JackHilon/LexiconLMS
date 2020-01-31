using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LexiconLMS.Data;
using LexiconLMS.Models;

namespace LexiconLMS.Controllers
{
    public class ModuleActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModuleActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ModuleActivities
        public async Task<IActionResult> Index()
        {
            return View(await _context.ModuleActivity.ToListAsync());
        }

        // GET: ModuleActivities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moduleActivity = await _context.ModuleActivity
                .FirstOrDefaultAsync(m => m.Id == id);
            if (moduleActivity == null)
            {
                return NotFound();
            }

            return View(moduleActivity);
        }

        // GET: ModuleActivities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ModuleActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartDate,EndDate")] ModuleActivity moduleActivity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(moduleActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(moduleActivity);
        }

        // GET: ModuleActivities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moduleActivity = await _context.ModuleActivity.FindAsync(id);
            if (moduleActivity == null)
            {
                return NotFound();
            }
            return View(moduleActivity);
        }

        // POST: ModuleActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,EndDate")] ModuleActivity moduleActivity)
        {
            if (id != moduleActivity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(moduleActivity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleActivityExists(moduleActivity.Id))
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
            return View(moduleActivity);
        }

        // GET: ModuleActivities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moduleActivity = await _context.ModuleActivity
                .FirstOrDefaultAsync(m => m.Id == id);
            if (moduleActivity == null)
            {
                return NotFound();
            }

            return View(moduleActivity);
        }

        // POST: ModuleActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var moduleActivity = await _context.ModuleActivity.FindAsync(id);
            _context.ModuleActivity.Remove(moduleActivity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleActivityExists(int id)
        {
            return _context.ModuleActivity.Any(e => e.Id == id);
        }
    }
}
