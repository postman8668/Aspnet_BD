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
    public class ReaderAttributesController : Controller
    {
        private readonly LibSysDbContext _context;

        public ReaderAttributesController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: ReaderAttributes
        public async Task<IActionResult> Index()
        {
            var manufacturingDbContext = _context.ReaderAttributes.Include(r => r.Reader);
            return View(await manufacturingDbContext.ToListAsync());
        }

        // GET: ReaderAttributes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readerAttribute = await _context.ReaderAttributes
                .Include(r => r.Reader)
                .FirstOrDefaultAsync(m => m.AttributeId == id);
            if (readerAttribute == null)
            {
                return NotFound();
            }

            return View(readerAttribute);
        }

        // GET: ReaderAttributes/Create
        public IActionResult Create()
        {
            ViewData["ReaderId"] = new SelectList(_context.Readers, "ReaderId", "LastName");
            return View();
        }

        // POST: ReaderAttributes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttributeId,ReaderId,AttributeName,AttributeValue")] ReaderAttribute readerAttribute)
        {
            if (ModelState.IsValid)
            {
                _context.Add(readerAttribute);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReaderId"] = new SelectList(_context.Readers, "ReaderId", "LastName", readerAttribute.ReaderId);
            return View(readerAttribute);
        }

        // GET: ReaderAttributes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readerAttribute = await _context.ReaderAttributes.FindAsync(id);
            if (readerAttribute == null)
            {
                return NotFound();
            }
            ViewData["ReaderId"] = new SelectList(_context.Readers, "ReaderId", "LastName", readerAttribute.ReaderId);
            return View(readerAttribute);
        }

        // POST: ReaderAttributes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AttributeId,ReaderId,AttributeName,AttributeValue")] ReaderAttribute readerAttribute)
        {
            if (id != readerAttribute.AttributeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(readerAttribute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReaderAttributeExists(readerAttribute.AttributeId))
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
            ViewData["ReaderId"] = new SelectList(_context.Readers, "ReaderId", "LastName", readerAttribute.ReaderId);
            return View(readerAttribute);
        }

        // GET: ReaderAttributes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readerAttribute = await _context.ReaderAttributes
                .Include(r => r.Reader)
                .FirstOrDefaultAsync(m => m.AttributeId == id);
            if (readerAttribute == null)
            {
                return NotFound();
            }

            return View(readerAttribute);
        }

        // POST: ReaderAttributes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var readerAttribute = await _context.ReaderAttributes.FindAsync(id);
            if (readerAttribute != null)
            {
                _context.ReaderAttributes.Remove(readerAttribute);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReaderAttributeExists(int id)
        {
            return _context.ReaderAttributes.Any(e => e.AttributeId == id);
        }
    }
}
