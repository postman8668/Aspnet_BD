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
    public class ContentsController : Controller
    {
        private readonly LibSysDbContext _context;

        public ContentsController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: Contents
        public async Task<IActionResult> Index()
        {
            var manufacturingDbContext = _context.Contents.Include(c => c.Publication).Include(c => c.Work);
            return View(await manufacturingDbContext.ToListAsync());
        }

        // GET: Contents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents
                .Include(c => c.Publication)
                .Include(c => c.Work)
                .FirstOrDefaultAsync(m => m.ContentId == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // GET: Contents/Create
        public IActionResult Create()
        {
            ViewData["PublicationId"] = new SelectList(_context.Publications, "PublicationId", "Title");
            ViewData["WorkId"] = new SelectList(_context.Works, "WorkId", "Title");
            return View();
        }

        // POST: Contents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContentId,PublicationId,WorkId,PageStart,PageEnd")] Content content)
        {
            if (ModelState.IsValid)
            {
                _context.Add(content);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublicationId"] = new SelectList(_context.Publications, "PublicationId", "Title", content.PublicationId);
            ViewData["WorkId"] = new SelectList(_context.Works, "WorkId", "Title", content.WorkId);
            return View(content);
        }

        // GET: Contents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents.FindAsync(id);
            if (content == null)
            {
                return NotFound();
            }
            ViewData["PublicationId"] = new SelectList(_context.Publications, "PublicationId", "Title", content.PublicationId);
            ViewData["WorkId"] = new SelectList(_context.Works, "WorkId", "Title", content.WorkId);
            return View(content);
        }

        // POST: Contents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContentId,PublicationId,WorkId,PageStart,PageEnd")] Content content)
        {
            if (id != content.ContentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(content);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentExists(content.ContentId))
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
            ViewData["PublicationId"] = new SelectList(_context.Publications, "PublicationId", "Title", content.PublicationId);
            ViewData["WorkId"] = new SelectList(_context.Works, "WorkId", "Title", content.WorkId);
            return View(content);
        }

        // GET: Contents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _context.Contents
                .Include(c => c.Publication)
                .Include(c => c.Work)
                .FirstOrDefaultAsync(m => m.ContentId == id);
            if (content == null)
            {
                return NotFound();
            }

            return View(content);
        }

        // POST: Contents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content != null)
            {
                _context.Contents.Remove(content);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContentExists(int id)
        {
            return _context.Contents.Any(e => e.ContentId == id);
        }
    }
}
