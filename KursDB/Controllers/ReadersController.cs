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
    public class ReadersController : Controller
    {
        private readonly LibSysDbContext _context;

        public ReadersController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: Readers
        public async Task<IActionResult> Index()
        {
            var readers = await _context.Readers
                .Include(r => r.Library)
                .Include(r => r.ReaderType)
                .ToListAsync();
            return View(readers);
        }

        // GET: Readers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers
                .Include(r => r.Library)
                .Include(r => r.ReaderType)
                .FirstOrDefaultAsync(m => m.ReaderId == id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // GET: Readers/Create
        public IActionResult Create()
        {
            ViewData["Libraries"] = new SelectList(_context.Libraries, "LibraryId", "Name");
            ViewData["ReaderTypes"] = new SelectList(_context.ReaderTypes, "ReaderTypeId", "TypeName");
            return View();
        }

        // POST: Readers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReaderId,LastName,FirstName,MiddleName,BirthDate,Address,Phone,Email,RegistrationDate,LibraryId,ReaderTypeId")] Reader reader)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reader);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Libraries"] = new SelectList(_context.Libraries, "LibraryId", "Name", reader.LibraryId);
            ViewData["ReaderTypes"] = new SelectList(_context.ReaderTypes, "ReaderTypeId", "TypeName", reader.ReaderTypeId);
            return View(reader);
        }

        // GET: Readers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers.FindAsync(id);
            if (reader == null)
            {
                return NotFound();
            }
            ViewData["Libraries"] = new SelectList(_context.Libraries, "LibraryId", "Name", reader.LibraryId);
            ViewData["ReaderTypes"] = new SelectList(_context.ReaderTypes, "ReaderTypeId", "TypeName", reader.ReaderTypeId);
            return View(reader);
        }

        // POST: Readers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReaderId,LastName,FirstName,MiddleName,BirthDate,Address,Phone,Email,RegistrationDate,LibraryId,ReaderTypeId")] Reader reader)
        {
            if (id != reader.ReaderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reader);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReaderExists(reader.ReaderId))
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
            ViewData["Libraries"] = new SelectList(_context.Libraries, "LibraryId", "Name", reader.LibraryId);
            ViewData["ReaderTypes"] = new SelectList(_context.ReaderTypes, "ReaderTypeId", "TypeName", reader.ReaderTypeId);
            return View(reader);
        }

        // GET: Readers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reader = await _context.Readers
                .Include(r => r.Library)
                .Include(r => r.ReaderType)
                .FirstOrDefaultAsync(m => m.ReaderId == id);
            if (reader == null)
            {
                return NotFound();
            }

            return View(reader);
        }

        // POST: Readers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reader = await _context.Readers.FindAsync(id);
            if (reader != null)
            {
                _context.Readers.Remove(reader);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReaderExists(int id)
        {
            return _context.Readers.Any(e => e.ReaderId == id);
        }
    }
}