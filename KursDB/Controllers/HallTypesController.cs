using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KursDB.Data;
using KursDB.Models;

namespace KursDB.Controllers
{
    public class HallTypesController : Controller
    {
        private readonly LibSysDbContext _context;

        public HallTypesController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: HallTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.HallTypes.ToListAsync());
        }

        // GET: HallTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hallType = await _context.HallTypes
                .FirstOrDefaultAsync(m => m.HallTypeId == id);
            if (hallType == null)
            {
                return NotFound();
            }

            return View(hallType);
        }

        // GET: HallTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HallTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HallTypeId,TypeName")] HallType hallType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hallType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hallType);
        }

        // GET: HallTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hallType = await _context.HallTypes.FindAsync(id);
            if (hallType == null)
            {
                return NotFound();
            }
            return View(hallType);
        }

        // POST: HallTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HallTypeId,TypeName")] HallType hallType)
        {
            if (id != hallType.HallTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hallType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HallTypeExists(hallType.HallTypeId))
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
            return View(hallType);
        }

        // GET: HallTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hallType = await _context.HallTypes
                .Include(ht => ht.Halls)
                .FirstOrDefaultAsync(m => m.HallTypeId == id);

            if (hallType == null)
            {
                return NotFound();
            }

            // Проверка на связанные залы
            ViewBag.HallCount = hallType.Halls?.Count ?? 0;
            return View(hallType);
        }

        // POST: HallTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Находим тип зала со всеми зависимостями
                    var hallType = await _context.HallTypes
                        .Include(ht => ht.Halls)
                            .ThenInclude(h => h.EmployeeSchedules)
                        .Include(ht => ht.Halls)
                            .ThenInclude(h => h.Inventories)
                        .FirstOrDefaultAsync(ht => ht.HallTypeId == id);

                    if (hallType == null)
                    {
                        return NotFound();
                    }

                    // 2. Обрабатываем каждый зал этого типа
                    foreach (var hall in hallType.Halls.ToList())
                    {
                        // Удаляем расписания сотрудников для этого зала
                        _context.EmployeeSchedules.RemoveRange(hall.EmployeeSchedules);

                        // Обрабатываем инвентарь в зале
                        foreach (var inventory in hall.Inventories.ToList())
                        {
                            // Возвращаем все выданные книги из этого инвентаря
                            var loans = await _context.Loans
                                .Where(l => l.InventoryId == inventory.InventoryId && l.ReturnDate == null)
                                .ToListAsync();

                            foreach (var loan in loans)
                            {
                                loan.ReturnDate = DateTime.Now;
                            }
                        }

                        // Удаляем инвентарь зала
                        _context.Inventories.RemoveRange(hall.Inventories);

                        // Удаляем сам зал
                        _context.Halls.Remove(hall);
                    }

                    // 3. Удаляем сам тип зала
                    _context.HallTypes.Remove(hallType);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"Не удалось удалить тип зала: {ex.Message}");
                    return await Delete(id);
                }
            }
        }

        private bool HallTypeExists(int id)
        {
            return _context.HallTypes.Any(e => e.HallTypeId == id);
        }
    }
}