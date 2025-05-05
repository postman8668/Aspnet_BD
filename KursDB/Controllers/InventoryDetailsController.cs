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
    public class InventoryDetailsController : Controller
    {
        private readonly LibSysDbContext _context; // Замените на ваш реальный DbContext

        public InventoryDetailsController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: InventoryDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.InventoryDetails.ToListAsync());
        }

        // GET: InventoryDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryDetail = await _context.InventoryDetails
                .FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventoryDetail == null)
            {
                return NotFound();
            }

            return View(inventoryDetail);
        }

        // GET: InventoryDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InventoryDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryId,PublicationTitle,PublicationType,LibraryName,HallNumber,HallType,RackNumber,ShelfNumber,AcquisitionDate,WriteOffDate,Status,CanTakeHome,LoanPeriodDays")] InventoryDetail inventoryDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventoryDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventoryDetail);
        }

        // GET: InventoryDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryDetail = await _context.InventoryDetails.FindAsync(id);
            if (inventoryDetail == null)
            {
                return NotFound();
            }
            return View(inventoryDetail);
        }

        // POST: InventoryDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryId,PublicationTitle,PublicationType,LibraryName,HallNumber,HallType,RackNumber,ShelfNumber,AcquisitionDate,WriteOffDate,Status,CanTakeHome,LoanPeriodDays")] InventoryDetail inventoryDetail)
        {
            if (id != inventoryDetail.InventoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventoryDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryDetailExists(inventoryDetail.InventoryId))
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
            return View(inventoryDetail);
        }

        // GET: InventoryDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryDetail = await _context.InventoryDetails
                .FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventoryDetail == null)
            {
                return NotFound();
            }

            return View(inventoryDetail);
        }

        // POST: InventoryDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventoryDetail = await _context.InventoryDetails.FindAsync(id);
            if (inventoryDetail != null)
            {
                _context.InventoryDetails.Remove(inventoryDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryDetailExists(int id)
        {
            return _context.InventoryDetails.Any(e => e.InventoryId == id);
        }
    }
}