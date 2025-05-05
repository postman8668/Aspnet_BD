using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KursDB.Data;
using KursDB.Models;

namespace KursDB.Controllers
{
    public class CurrentLoansController : Controller
    {
        private readonly LibSysDbContext _context;

        public CurrentLoansController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: CurrentLoans
        public async Task<IActionResult> Index()
        {
            return View(await _context.CurrentLoans.ToListAsync());
        }

        // GET: CurrentLoans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentLoan = await _context.CurrentLoans
                .FirstOrDefaultAsync(m => m.LoanId == id);
            if (currentLoan == null)
            {
                return NotFound();
            }

            return View(currentLoan);
        }

        // GET: CurrentLoans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CurrentLoans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoanId,ReaderName,PublicationTitle,EmployeeName,LoanDate,DueDate,DaysOverdue,LibraryName")] CurrentLoan currentLoan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(currentLoan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(currentLoan);
        }

        // GET: CurrentLoans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentLoan = await _context.CurrentLoans.FindAsync(id);
            if (currentLoan == null)
            {
                return NotFound();
            }
            return View(currentLoan);
        }

        // POST: CurrentLoans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoanId,ReaderName,PublicationTitle,EmployeeName,LoanDate,DueDate,DaysOverdue,LibraryName")] CurrentLoan currentLoan)
        {
            if (id != currentLoan.LoanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(currentLoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CurrentLoanExists(currentLoan.LoanId))
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
            return View(currentLoan);
        }

        // GET: CurrentLoans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentLoan = await _context.CurrentLoans
                .FirstOrDefaultAsync(m => m.LoanId == id);
            if (currentLoan == null)
            {
                return NotFound();
            }

            return View(currentLoan);
        }

        // POST: CurrentLoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentLoan = await _context.CurrentLoans.FindAsync(id);
            if (currentLoan != null)
            {
                _context.CurrentLoans.Remove(currentLoan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CurrentLoanExists(int id)
        {
            return _context.CurrentLoans.Any(e => e.LoanId == id);
        }
    }
}
