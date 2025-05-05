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
    public class AuthorshipsController : Controller
    {
        private readonly LibSysDbContext _context;

        public AuthorshipsController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: Authorships
        public async Task<IActionResult> Index()
        {
            var manufacturingDbContext = _context.Authorships.Include(a => a.Author).Include(a => a.Work);
            return View(await manufacturingDbContext.ToListAsync());
        }

        // GET: Authorships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorship = await _context.Authorships
                .Include(a => a.Author)
                .Include(a => a.Work)
                .FirstOrDefaultAsync(m => m.AuthorshipId == id);
            if (authorship == null)
            {
                return NotFound();
            }

            return View(authorship);
        }

        // GET: Authorships/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "LastName");
            ViewData["WorkId"] = new SelectList(_context.Works, "WorkId", "Title");
            return View();
        }

        // POST: Authorships/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorshipId,AuthorId,WorkId")] Authorship authorship)
        {
            if (ModelState.IsValid)
            {
                _context.Add(authorship);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "LastName", authorship.AuthorId);
            ViewData["WorkId"] = new SelectList(_context.Works, "WorkId", "Title", authorship.WorkId);
            return View(authorship);
        }

        // GET: Authorships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorship = await _context.Authorships.FindAsync(id);
            if (authorship == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "LastName", authorship.AuthorId);
            ViewData["WorkId"] = new SelectList(_context.Works, "WorkId", "Title", authorship.WorkId);
            return View(authorship);
        }

        // POST: Authorships/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorshipId,AuthorId,WorkId")] Authorship authorship)
        {
            if (id != authorship.AuthorshipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(authorship);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorshipExists(authorship.AuthorshipId))
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
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "LastName", authorship.AuthorId);
            ViewData["WorkId"] = new SelectList(_context.Works, "WorkId", "Title", authorship.WorkId);
            return View(authorship);
        }

        // GET: Authorships/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorship = await _context.Authorships
                .Include(a => a.Author)
                .Include(a => a.Work)
                .FirstOrDefaultAsync(m => m.AuthorshipId == id);
            if (authorship == null)
            {
                return NotFound();
            }

            return View(authorship);
        }

        // POST: Authorships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var authorship = await _context.Authorships.FindAsync(id);
            if (authorship != null)
            {
                _context.Authorships.Remove(authorship);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorshipExists(int id)
        {
            return _context.Authorships.Any(e => e.AuthorshipId == id);
        }
    }
}
