using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KursDB.Data;
using KursDB.Models;

namespace KursDB.Controllers
{
    public class LibrariesController : Controller
    {
        private readonly LibSysDbContext _context;

        public LibrariesController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: Libraries
        public async Task<IActionResult> Index()
        {
            return View(await _context.Libraries.ToListAsync());
        }

        // GET: Libraries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var library = await _context.Libraries
                .FirstOrDefaultAsync(m => m.LibraryId == id);
            if (library == null)
            {
                return NotFound();
            }

            return View(library);
        }

        // GET: Libraries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Libraries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LibraryId,Name,Address,Phone,Email")] Library library)
        {
            if (ModelState.IsValid)
            {
                _context.Add(library);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(library);
        }

        // GET: Libraries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var library = await _context.Libraries.FindAsync(id);
            if (library == null)
            {
                return NotFound();
            }
            return View(library);
        }

        // POST: Libraries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LibraryId,Name,Address,Phone,Email")] Library library)
        {
            if (id != library.LibraryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(library);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryExists(library.LibraryId))
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
            return View(library);
        }

        // GET: Libraries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var library = await _context.Libraries
                .Include(l => l.Employees)
                .Include(l => l.Halls)
                .Include(l => l.Readers)
                .Include(l => l.Inventories)
                .FirstOrDefaultAsync(m => m.LibraryId == id);

            if (library == null)
            {
                return NotFound();
            }

            ViewBag.EmployeeCount = library.Employees?.Count ?? 0;
            ViewBag.HallCount = library.Halls?.Count ?? 0;
            ViewBag.ReaderCount = library.Readers?.Count ?? 0;
            ViewBag.InventoryCount = library.Inventories?.Count ?? 0;

            return View(library);
        }

        // POST: Libraries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var library = await _context.Libraries
                        .Include(l => l.Employees)
                            .ThenInclude(e => e.EmployeeSchedules)
                        .Include(l => l.Employees)
                            .ThenInclude(e => e.Loans)
                        .Include(l => l.Halls)
                            .ThenInclude(h => h.EmployeeSchedules)
                        .Include(l => l.Halls)
                            .ThenInclude(h => h.Inventories)
                                .ThenInclude(i => i.Loans)
                        .Include(l => l.Readers)
                            .ThenInclude(r => r.ReaderAttributes)
                        .Include(l => l.Readers)
                            .ThenInclude(r => r.Loans)
                        .Include(l => l.Inventories)
                            .ThenInclude(i => i.Loans)
                        .FirstOrDefaultAsync(l => l.LibraryId == id);

                    if (library == null)
                    {
                        return NotFound();
                    }

                    // 1. Обработка сотрудников библиотеки
                    foreach (var employee in library.Employees.ToList())
                    {
                        // Удаление расписаний сотрудника
                        _context.EmployeeSchedules.RemoveRange(employee.EmployeeSchedules);

                        // Возврат всех книг, выданных сотрудником
                        foreach (var loan in employee.Loans.Where(l => l.ReturnDate == null))
                        {
                            loan.ReturnDate = DateTime.Now;
                            if (loan.Inventory != null)
                            {
                                loan.Inventory.IsAvailable = true;
                            }
                        }

                        // Удаление сотрудника
                        _context.Employees.Remove(employee);
                    }

                    // 2. Обработка залов библиотеки
                    foreach (var hall in library.Halls.ToList())
                    {
                        // Удаление расписаний для зала
                        _context.EmployeeSchedules.RemoveRange(hall.EmployeeSchedules);

                        // Обработка инвентаря в зале
                        foreach (var inventory in hall.Inventories.ToList())
                        {
                            // Возврат всех выданных книг из этого инвентаря
                            foreach (var loan in inventory.Loans.Where(l => l.ReturnDate == null))
                            {
                                loan.ReturnDate = DateTime.Now;
                            }
                            _context.Inventories.Remove(inventory);
                        }

                        // Удаление зала
                        _context.Halls.Remove(hall);
                    }

                    // 3. Обработка читателей библиотеки
                    foreach (var reader in library.Readers.ToList())
                    {
                        // Удаление атрибутов читателя
                        _context.ReaderAttributes.RemoveRange(reader.ReaderAttributes);

                        // Возврат всех книг читателя
                        foreach (var loan in reader.Loans.Where(l => l.ReturnDate == null))
                        {
                            loan.ReturnDate = DateTime.Now;
                            if (loan.Inventory != null)
                            {
                                loan.Inventory.IsAvailable = true;
                            }
                        }

                        // Удаление читателя
                        _context.Readers.Remove(reader);
                    }

                    // 4. Обработка инвентаря библиотеки
                    foreach (var inventory in library.Inventories.ToList())
                    {
                        // Возврат всех выданных книг из этого инвентаря
                        foreach (var loan in inventory.Loans.Where(l => l.ReturnDate == null))
                        {
                            loan.ReturnDate = DateTime.Now;
                        }
                        _context.Inventories.Remove(inventory);
                    }

                    // 5. Удаление самой библиотеки
                    _context.Libraries.Remove(library);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"Не удалось удалить библиотеку: {ex.Message}");
                    return await Delete(id);
                }
            }
        }

        private bool LibraryExists(int id)
        {
            return _context.Libraries.Any(e => e.LibraryId == id);
        }
    }
}