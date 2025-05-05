using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Добавьте эту директиву
using Microsoft.EntityFrameworkCore;
using KursDB.Data;
using KursDB.Models;

namespace KursDB.Controllers
{
    public class HallsController : Controller
    {
        private readonly LibSysDbContext _context;

        public HallsController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: Halls
        public async Task<IActionResult> Index()
        {
            var halls = await _context.Halls
                .Include(h => h.HallType)
                .Include(h => h.Library)
                .ToListAsync();
            return View(halls);
        }

        // GET: Halls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls
                .Include(h => h.HallType)
                .Include(h => h.Library)
                .Include(h => h.EmployeeSchedules)
                    .ThenInclude(es => es.Employee)
                .Include(h => h.Inventories)
                .FirstOrDefaultAsync(m => m.HallId == id);

            if (hall == null)
            {
                return NotFound();
            }

            return View(hall);
        }

        // GET: Halls/Create
        public IActionResult Create()
        {
            ViewData["HallTypeId"] = new SelectList(_context.HallTypes, "HallTypeId", "TypeName");
            ViewData["LibraryId"] = new SelectList(_context.Libraries, "LibraryId", "Name");
            return View();
        }

        // POST: Halls/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HallId,LibraryId,HallTypeId,HallNumber,Capacity")] Hall hall)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HallTypeId"] = new SelectList(_context.HallTypes, "HallTypeId", "TypeName", hall.HallTypeId);
            ViewData["LibraryId"] = new SelectList(_context.Libraries, "LibraryId", "Name", hall.LibraryId);
            return View(hall);
        }

        // GET: Halls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls.FindAsync(id);
            if (hall == null)
            {
                return NotFound();
            }
            ViewData["HallTypeId"] = new SelectList(_context.HallTypes, "HallTypeId", "TypeName", hall.HallTypeId);
            ViewData["LibraryId"] = new SelectList(_context.Libraries, "LibraryId", "Name", hall.LibraryId);
            return View(hall);
        }

        // POST: Halls/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HallId,LibraryId,HallTypeId,HallNumber,Capacity")] Hall hall)
        {
            if (id != hall.HallId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hall);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HallExists(hall.HallId))
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
            ViewData["HallTypeId"] = new SelectList(_context.HallTypes, "HallTypeId", "TypeName", hall.HallTypeId);
            ViewData["LibraryId"] = new SelectList(_context.Libraries, "LibraryId", "Name", hall.LibraryId);
            return View(hall);
        }

        // GET: Halls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hall = await _context.Halls
                .Include(h => h.HallType)
                .Include(h => h.Library)
                .Include(h => h.EmployeeSchedules)
                .Include(h => h.Inventories)
                .FirstOrDefaultAsync(m => m.HallId == id);

            if (hall == null)
            {
                return NotFound();
            }

            ViewBag.ScheduleCount = hall.EmployeeSchedules?.Count ?? 0;
            ViewBag.InventoryCount = hall.Inventories?.Count ?? 0;

            return View(hall);
        }

        // POST: Halls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var hall = await _context.Halls
                        .Include(h => h.EmployeeSchedules)
                        .Include(h => h.Inventories)
                            .ThenInclude(i => i.Loans)
                        .FirstOrDefaultAsync(h => h.HallId == id);

                    if (hall == null)
                    {
                        return NotFound();
                    }

                    // 1. Удаление расписаний сотрудников для этого зала
                    _context.EmployeeSchedules.RemoveRange(hall.EmployeeSchedules);

                    // 2. Обработка инвентаря в зале
                    foreach (var inventory in hall.Inventories.ToList())
                    {
                        // Возврат всех выданных книг из этого инвентаря
                        foreach (var loan in inventory.Loans.Where(l => l.ReturnDate == null))
                        {
                            loan.ReturnDate = DateTime.Now;
                        }
                        _context.Inventories.Remove(inventory);
                    }

                    // 3. Удаление самого зала
                    _context.Halls.Remove(hall);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"Не удалось удалить зал: {ex.Message}");
                    return await Delete(id);
                }
            }
        }

        private bool HallExists(int id)
        {
            return _context.Halls.Any(e => e.HallId == id);
        }
    }
}