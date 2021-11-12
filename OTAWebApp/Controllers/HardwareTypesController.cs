using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OTAWebApp.Data;
using OTAWebApp.Models;

namespace OTAWebApp.Controllers
{
    public class HardwareTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HardwareTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HardwareTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.HardwareType.ToListAsync());
        }

        // GET: HardwareTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hardwareType = await _context.HardwareType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hardwareType == null)
            {
                return NotFound();
            }

            return View(hardwareType);
        }

        // GET: HardwareTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HardwareTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] HardwareType hardwareType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hardwareType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hardwareType);
        }

        // GET: HardwareTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hardwareType = await _context.HardwareType.FindAsync(id);
            if (hardwareType == null)
            {
                return NotFound();
            }
            return View(hardwareType);
        }

        // POST: HardwareTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] HardwareType hardwareType)
        {
            if (id != hardwareType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hardwareType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HardwareTypeExists(hardwareType.Id))
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
            return View(hardwareType);
        }

        // GET: HardwareTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hardwareType = await _context.HardwareType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hardwareType == null)
            {
                return NotFound();
            }

            return View(hardwareType);
        }

        // POST: HardwareTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hardwareType = await _context.HardwareType.FindAsync(id);
            _context.HardwareType.Remove(hardwareType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HardwareTypeExists(int id)
        {
            return _context.HardwareType.Any(e => e.Id == id);
        }
    }
}
