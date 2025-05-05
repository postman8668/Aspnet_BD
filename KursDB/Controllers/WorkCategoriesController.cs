using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KursDB.Data;
using KursDB.Models;

namespace KursDB.Controllers
{
    public class WorkCategoriesController : Controller
    {
        private readonly LibSysDbContext _context;

        public WorkCategoriesController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: WorkCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkCategories.ToListAsync());
        }

        // GET: WorkCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workCategory = await _context.WorkCategories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (workCategory == null)
            {
                return NotFound();
            }

            return View(workCategory);
        }

        // GET: WorkCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName")] WorkCategory workCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workCategory);
        }

        // GET: WorkCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workCategory = await _context.WorkCategories.FindAsync(id);
            if (workCategory == null)
            {
                return NotFound();
            }
            return View(workCategory);
        }

        // POST: WorkCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName")] WorkCategory workCategory)
        {
            if (id != workCategory.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkCategoryExists(workCategory.CategoryId))
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
            return View(workCategory);
        }

        // GET: WorkCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workCategory = await _context.WorkCategories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (workCategory == null)
            {
                return NotFound();
            }

            return View(workCategory);
        }

        // POST: WorkCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workCategory = await _context.WorkCategories.FindAsync(id);
            if (workCategory != null)
            {
                _context.WorkCategories.Remove(workCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkCategoryExists(int id)
        {
            return _context.WorkCategories.Any(e => e.CategoryId == id);
        }
    }
}
