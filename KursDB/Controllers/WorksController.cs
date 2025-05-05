using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KursDB.Data;
using KursDB.Models;

namespace KursDB.Controllers
{
    public class WorksController : Controller
    {
        private readonly LibSysDbContext _context;

        public WorksController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: Works
        public async Task<IActionResult> Index()
        {
            var works = await _context.Works
                .Include(w => w.Category)
                .ToListAsync();
            return View(works);
        }

        // GET: Works/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var work = await _context.Works
                .Include(w => w.Category)
                .Include(w => w.Authorships)
                    .ThenInclude(a => a.Author)
                .Include(w => w.Contents)
                    .ThenInclude(c => c.Publication)
                .FirstOrDefaultAsync(m => m.WorkId == id);

            if (work == null)
            {
                return NotFound();
            }

            return View(work);
        }

        // GET: Works/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.WorkCategories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Works/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkId,Title,CategoryId,YearWritten,Description")] Work work)
        {
            if (ModelState.IsValid)
            {
                _context.Add(work);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.WorkCategories, "CategoryId", "CategoryName", work.CategoryId);
            return View(work);
        }

        // GET: Works/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var work = await _context.Works.FindAsync(id);
            if (work == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.WorkCategories, "CategoryId", "CategoryName", work.CategoryId);
            return View(work);
        }

        // POST: Works/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkId,Title,CategoryId,YearWritten,Description")] Work work)
        {
            if (id != work.WorkId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(work);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkExists(work.WorkId))
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
            ViewData["CategoryId"] = new SelectList(_context.WorkCategories, "CategoryId", "CategoryName", work.CategoryId);
            return View(work);
        }

        // GET: Works/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var work = await _context.Works
                .Include(w => w.Category)
                .Include(w => w.Authorships)
                .Include(w => w.Contents)
                .FirstOrDefaultAsync(m => m.WorkId == id);

            if (work == null)
            {
                return NotFound();
            }

            // Проверяем наличие связанных записей
            var authorshipCount = work.Authorships?.Count ?? 0;
            var contentCount = work.Contents?.Count ?? 0;

            ViewBag.AuthorshipCount = authorshipCount;
            ViewBag.ContentCount = contentCount;
            ViewBag.CanDelete = (authorshipCount == 0 && contentCount == 0);

            return View(work);
        }

        // POST: Works/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var work = await _context.Works
                        .Include(w => w.Authorships)
                        .Include(w => w.Contents)
                        .FirstOrDefaultAsync(w => w.WorkId == id);

                    if (work == null)
                    {
                        return NotFound();
                    }

                    // 1. Удаление связей с авторами (Authorships)
                    if (work.Authorships != null && work.Authorships.Any())
                    {
                        _context.Authorships.RemoveRange(work.Authorships);
                    }

                    // 2. Удаление связей с публикациями (Contents)
                    // Важно: из-за ON DELETE NO ACTION удаляем вручную
                    var contents = await _context.Contents
                        .Where(c => c.WorkId == id)
                        .ToListAsync();

                    if (contents != null && contents.Any())
                    {
                        _context.Contents.RemoveRange(contents);
                    }

                    // 3. Удаление самого произведения
                    _context.Works.Remove(work);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    // Загружаем данные для повторного отображения формы
                    var work = await _context.Works
                        .Include(w => w.Category)
                        .Include(w => w.Authorships)
                        .Include(w => w.Contents)
                        .FirstOrDefaultAsync(w => w.WorkId == id);

                    if (work != null)
                    {
                        ViewBag.AuthorshipCount = work.Authorships?.Count ?? 0;
                        ViewBag.ContentCount = work.Contents?.Count ?? 0;
                        ViewBag.CanDelete = false;
                    }

                    ModelState.AddModelError("", $"Не удалось удалить произведение: {ex.Message}");
                    return View("Delete", work);
                }
            }
        }

        private bool WorkExists(int id)
        {
            return _context.Works.Any(e => e.WorkId == id);
        }
    }
}