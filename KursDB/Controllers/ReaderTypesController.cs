using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KursDB.Data;
using KursDB.Models;

namespace KursDB.Controllers
{
    public class ReaderTypesController : Controller
    {
        private readonly LibSysDbContext _context;

        public ReaderTypesController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: ReaderTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReaderTypes.ToListAsync());
        }

        // GET: ReaderTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readerType = await _context.ReaderTypes
                .FirstOrDefaultAsync(m => m.ReaderTypeId == id);
            if (readerType == null)
            {
                return NotFound();
            }

            return View(readerType);
        }

        // GET: ReaderTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReaderTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReaderTypeId,TypeName")] ReaderType readerType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(readerType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(readerType);
        }

        // GET: ReaderTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readerType = await _context.ReaderTypes.FindAsync(id);
            if (readerType == null)
            {
                return NotFound();
            }
            return View(readerType);
        }

        // POST: ReaderTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReaderTypeId,TypeName")] ReaderType readerType)
        {
            if (id != readerType.ReaderTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(readerType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReaderTypeExists(readerType.ReaderTypeId))
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
            return View(readerType);
        }

        // GET: ReaderTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readerType = await _context.ReaderTypes
                .Include(rt => rt.Readers)
                .FirstOrDefaultAsync(m => m.ReaderTypeId == id);

            if (readerType == null)
            {
                return NotFound();
            }

            ViewBag.ReaderCount = readerType.Readers.Count;
            return View(readerType);
        }

        // POST: ReaderTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var readerType = await _context.ReaderTypes
                        .Include(rt => rt.Readers)
                            .ThenInclude(r => r.ReaderAttributes)
                        .Include(rt => rt.Readers)
                            .ThenInclude(r => r.Loans)
                                .ThenInclude(l => l.Inventory)
                        .FirstOrDefaultAsync(rt => rt.ReaderTypeId == id);

                    if (readerType == null)
                    {
                        return NotFound();
                    }

                    // Обработка всех связанных читателей
                    foreach (var reader in readerType.Readers.ToList())
                    {
                        // Удаление атрибутов читателя
                        _context.ReaderAttributes.RemoveRange(reader.ReaderAttributes);

                        // Возврат всех книг
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

                    // Удаление самого типа читателя
                    _context.ReaderTypes.Remove(readerType);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"Error deleting reader type: {ex.Message}");
                    return await Delete(id);
                }
            }
        }

        private bool ReaderTypeExists(int id)
        {
            return _context.ReaderTypes.Any(e => e.ReaderTypeId == id);
        }
    }
}