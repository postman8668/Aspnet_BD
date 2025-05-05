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
    public class LoansController : Controller
    {
        private readonly LibSysDbContext _context;

        public LoansController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var loans = await _context.Loans
                .Include(l => l.Employee)
                .Include(l => l.Inventory)
                    .ThenInclude(i => i.Publication)
                .Include(l => l.Reader)
                .ToListAsync();
            return View(loans);
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Employee)
                .Include(l => l.Inventory)
                    .ThenInclude(i => i.Publication)
                .Include(l => l.Reader)
                .FirstOrDefaultAsync(m => m.LoanId == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Create
        public IActionResult Create()
        {
            ViewData["Employees"] = new SelectList(_context.Employees, "EmployeeId", "LastName");
            ViewData["Inventories"] = new SelectList(_context.Inventories.Include(i => i.Publication), "InventoryId", "Publication.Title");
            ViewData["Readers"] = new SelectList(_context.Readers, "ReaderId", "LastName");
            return View();
        }

        // POST: Loans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoanId,InventoryId,ReaderId,EmployeeId,LoanDate,DueDate,ReturnDate")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Employees"] = new SelectList(_context.Employees, "EmployeeId", "LastName", loan.EmployeeId);
            ViewData["Inventories"] = new SelectList(_context.Inventories.Include(i => i.Publication), "InventoryId", "Publication.Title", loan.InventoryId);
            ViewData["Readers"] = new SelectList(_context.Readers, "ReaderId", "LastName", loan.ReaderId);
            return View(loan);
        }

        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            ViewData["Employees"] = new SelectList(_context.Employees, "EmployeeId", "LastName", loan.EmployeeId);
            ViewData["Inventories"] = new SelectList(_context.Inventories.Include(i => i.Publication), "InventoryId", "Publication.Title", loan.InventoryId);
            ViewData["Readers"] = new SelectList(_context.Readers, "ReaderId", "LastName", loan.ReaderId);
            return View(loan);
        }

        // POST: Loans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoanId,InventoryId,ReaderId,EmployeeId,LoanDate,DueDate,ReturnDate")] Loan loan)
        {
            if (id != loan.LoanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.LoanId))
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
            ViewData["Employees"] = new SelectList(_context.Employees, "EmployeeId", "LastName", loan.EmployeeId);
            ViewData["Inventories"] = new SelectList(_context.Inventories.Include(i => i.Publication), "InventoryId", "Publication.Title", loan.InventoryId);
            ViewData["Readers"] = new SelectList(_context.Readers, "ReaderId", "LastName", loan.ReaderId);
            return View(loan);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Employee)
                .Include(l => l.Inventory)
                    .ThenInclude(i => i.Publication)
                .Include(l => l.Reader)
                .FirstOrDefaultAsync(m => m.LoanId == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null)
            {
                _context.Loans.Remove(loan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.LoanId == id);
        }
    }
}