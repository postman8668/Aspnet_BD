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
    public class ReaderDetailsController : Controller
    {
        private readonly LibSysDbContext _context;

        public ReaderDetailsController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: ReaderDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReaderDetails.ToListAsync());
        }

        // GET: ReaderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readerDetail = await _context.ReaderDetails
                .FirstOrDefaultAsync(m => m.ReaderId == id);
            if (readerDetail == null)
            {
                return NotFound();
            }

            return View(readerDetail);
        }

        // GET: ReaderDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReaderDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReaderId,FullName,BirthDate,Address,Phone,Email,RegistrationDate,ReaderType,LibraryName,Attributes")] ReaderDetail readerDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(readerDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(readerDetail);
        }

        // GET: ReaderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readerDetail = await _context.ReaderDetails.FindAsync(id);
            if (readerDetail == null)
            {
                return NotFound();
            }
            return View(readerDetail);
        }

        // POST: ReaderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReaderId,FullName,BirthDate,Address,Phone,Email,RegistrationDate,ReaderType,LibraryName,Attributes")] ReaderDetail readerDetail)
        {
            if (id != readerDetail.ReaderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(readerDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReaderDetailExists(readerDetail.ReaderId))
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
            return View(readerDetail);
        }

        // GET: ReaderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readerDetail = await _context.ReaderDetails
                .FirstOrDefaultAsync(m => m.ReaderId == id);
            if (readerDetail == null)
            {
                return NotFound();
            }

            return View(readerDetail);
        }

        // POST: ReaderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var readerDetail = await _context.ReaderDetails.FindAsync(id);
            if (readerDetail != null)
            {
                _context.ReaderDetails.Remove(readerDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReaderDetailExists(int id)
        {
            return _context.ReaderDetails.Any(e => e.ReaderId == id);
        }
    }
}