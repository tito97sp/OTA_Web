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
    public class SoftwareVersionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SoftwareVersionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SoftwareVersions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SoftwareVersion.Include(s => s.SoftwareType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SoftwareVersions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareVersion = await _context.SoftwareVersion
                .Include(s => s.SoftwareType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (softwareVersion == null)
            {
                return NotFound();
            }

            return View(softwareVersion);
        }

        // GET: SoftwareVersions/Create
        public IActionResult Create(int id)
        {
            ViewData["SoftwareTypeId"] = new SelectList(_context.SoftwareType, "Id", "Id");
            return View();
        }

        // POST: SoftwareVersions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SoftwareTypeId,Major,Minor,Patch,Label,Author,Date,FirmwarePath")] SoftwareVersion softwareVersion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(softwareVersion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SoftwareTypeId"] = new SelectList(_context.SoftwareType, "Id", "Id", softwareVersion.SoftwareTypeId);
            return View(softwareVersion);
        }

        // GET: SoftwareVersions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareVersion = await _context.SoftwareVersion.FindAsync(id);
            if (softwareVersion == null)
            {
                return NotFound();
            }
            ViewData["SoftwareTypeId"] = new SelectList(_context.SoftwareType, "Id", "Id", softwareVersion.SoftwareTypeId);
            return View(softwareVersion);
        }

        // POST: SoftwareVersions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SoftwareTypeId,Major,Minor,Patch,Label,Author,Date,FirmwarePath")] SoftwareVersion softwareVersion)
        {
            if (id != softwareVersion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(softwareVersion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoftwareVersionExists(softwareVersion.Id))
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
            ViewData["SoftwareTypeId"] = new SelectList(_context.SoftwareType, "Id", "Id", softwareVersion.SoftwareTypeId);
            return View(softwareVersion);
        }

        // GET: SoftwareVersions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareVersion = await _context.SoftwareVersion
                .Include(s => s.SoftwareType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (softwareVersion == null)
            {
                return NotFound();
            }

            return View(softwareVersion);
        }

        // POST: SoftwareVersions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var softwareVersion = await _context.SoftwareVersion.FindAsync(id);
            _context.SoftwareVersion.Remove(softwareVersion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SoftwareVersionExists(int id)
        {
            return _context.SoftwareVersion.Any(e => e.Id == id);
        }
    }
}
