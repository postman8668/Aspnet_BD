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
    public class InventoriesController : Controller
    {
        private readonly LibSysDbContext _context;

        public InventoriesController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: Inventories
        public async Task<IActionResult> Index()
        {
            var inventories = await _context.Inventories
                .Include(i => i.Hall)
                .Include(i => i.Library)
                .Include(i => i.Publication)
                .ToListAsync();
            return View(inventories);
        }

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Hall)
                .Include(i => i.Library)
                .Include(i => i.Publication)
                .FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // GET: Inventories/Create
        public IActionResult Create()
        {
            ViewData["Halls"] = new SelectList(_context.Halls, "HallId", "HallNumber");
            ViewData["Libraries"] = new SelectList(_context.Libraries, "LibraryId", "Name");
            ViewData["Publications"] = new SelectList(_context.Publications, "PublicationId", "Title");
            return View();
        }

        // POST: Inventories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryId,PublicationId,LibraryId,HallId,ShelfNumber,RackNumber,AcquisitionDate,WriteOffDate,IsAvailable,CanTakeHome,LoanPeriodDays")] Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Halls"] = new SelectList(_context.Halls, "HallId", "HallNumber", inventory.HallId);
            ViewData["Libraries"] = new SelectList(_context.Libraries, "LibraryId", "Name", inventory.LibraryId);
            ViewData["Publications"] = new SelectList(_context.Publications, "PublicationId", "Title", inventory.PublicationId);
            return View(inventory);
        }

        // GET: Inventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }
            ViewData["Halls"] = new SelectList(_context.Halls, "HallId", "HallNumber", inventory.HallId);
            ViewData["Libraries"] = new SelectList(_context.Libraries, "LibraryId", "Name", inventory.LibraryId);
            ViewData["Publications"] = new SelectList(_context.Publications, "PublicationId", "Title", inventory.PublicationId);
            return View(inventory);
        }

        // POST: Inventories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryId,PublicationId,LibraryId,HallId,ShelfNumber,RackNumber,AcquisitionDate,WriteOffDate,IsAvailable,CanTakeHome,LoanPeriodDays")] Inventory inventory)
        {
            if (id != inventory.InventoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(inventory.InventoryId))
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
            ViewData["Halls"] = new SelectList(_context.Halls, "HallId", "HallNumber", inventory.HallId);
            ViewData["Libraries"] = new SelectList(_context.Libraries, "LibraryId", "Name", inventory.LibraryId);
            ViewData["Publications"] = new SelectList(_context.Publications, "PublicationId", "Title", inventory.PublicationId);
            return View(inventory);
        }

        // GET: Inventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Hall)
                .Include(i => i.Library)
                .Include(i => i.Publication)
                .FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.InventoryId == id);
        }
    }
}