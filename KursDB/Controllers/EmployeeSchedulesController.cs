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
    public class EmployeeSchedulesController : Controller
    {
        private readonly LibSysDbContext _context;

        public EmployeeSchedulesController(LibSysDbContext context)
        {
            _context = context;
        }

        // GET: EmployeeSchedules
        public async Task<IActionResult> Index()
        {
            var manufacturingDbContext = _context.EmployeeSchedules.Include(e => e.Employee).Include(e => e.Hall);
            return View(await manufacturingDbContext.ToListAsync());
        }

        // GET: EmployeeSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeSchedule = await _context.EmployeeSchedules
                .Include(e => e.Employee)
                .Include(e => e.Hall)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (employeeSchedule == null)
            {
                return NotFound();
            }

            return View(employeeSchedule);
        }

        // GET: EmployeeSchedules/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "LastName");
            ViewData["HallId"] = new SelectList(_context.Halls, "HallId", "HallNumber");
            return View();
        }

        // POST: EmployeeSchedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduleId,EmployeeId,HallId,WorkDate,StartTime,EndTime")] EmployeeSchedule employeeSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "LastName", employeeSchedule.EmployeeId);
            ViewData["HallId"] = new SelectList(_context.Halls, "HallId", "HallNumber", employeeSchedule.HallId);
            return View(employeeSchedule);
        }

        // GET: EmployeeSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeSchedule = await _context.EmployeeSchedules.FindAsync(id);
            if (employeeSchedule == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "LastName", employeeSchedule.EmployeeId);
            ViewData["HallId"] = new SelectList(_context.Halls, "HallId", "HallNumber", employeeSchedule.HallId);
            return View(employeeSchedule);
        }

        // POST: EmployeeSchedules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,EmployeeId,HallId,WorkDate,StartTime,EndTime")] EmployeeSchedule employeeSchedule)
        {
            if (id != employeeSchedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeScheduleExists(employeeSchedule.ScheduleId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "LastName", employeeSchedule.EmployeeId);
            ViewData["HallId"] = new SelectList(_context.Halls, "HallId", "HallNumber", employeeSchedule.HallId);
            return View(employeeSchedule);
        }

        // GET: EmployeeSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeSchedule = await _context.EmployeeSchedules
                .Include(e => e.Employee)
                .Include(e => e.Hall)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (employeeSchedule == null)
            {
                return NotFound();
            }

            return View(employeeSchedule);
        }

        // POST: EmployeeSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeSchedule = await _context.EmployeeSchedules.FindAsync(id);
            if (employeeSchedule != null)
            {
                _context.EmployeeSchedules.Remove(employeeSchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeScheduleExists(int id)
        {
            return _context.EmployeeSchedules.Any(e => e.ScheduleId == id);
        }
    }
}
