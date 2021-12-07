using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OTAWebApp.Data;
using OTAWebApp.Models;
using OTAWebApp.ViewModels;

namespace OTAWebApp.Controllers
{
    public class SoftwareTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SoftwareTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SoftwareTypes
        public async Task<IActionResult> Index(int? id, int projectId)
        {
            var viewModel = new SoftwareTypeIndexData();

            viewModel.SoftwareTypes = _context.SoftwareType
                .Include(i => i.Project)
                .Where(i=> i.Project.Id.Equals(projectId))
                .OrderBy(i => i.CreationDate);

            //if (id != null) 
            //{
            //    ViewBag.SoftwareTypeID = id.Value;                    
            //    viewModel.SoftwareVersions = viewModel.SoftwareTypes.Where(
            //        i => i.Id == id.Value).Single().SoftwareVersions;
            //}

            return View(viewModel);
        }


        // GET: SoftwareTypes/Details/5
        public async Task<IActionResult> Details(int? softwareTypeId)
        {
            var viewModel = new SoftwareTypeDetailsData();
            

            if (softwareTypeId != null)
            {
                viewModel.SoftwareType = _context.SoftwareType
                    .Include(i => i.SoftwareVersions)
                    .Where(i => i.Id == softwareTypeId)
                    .FirstOrDefault();

                viewModel.SoftwareVersions = Enumerable.Empty<SoftwareVersion>();
                if (viewModel.SoftwareType.SoftwareVersions != null)
                {
                    viewModel.SoftwareVersions = viewModel.SoftwareType.SoftwareVersions
                        .Where(i => i.SoftwareType.Id.Equals(softwareTypeId));
                }

                ViewBag.SoftwareTypeID = softwareTypeId;
                ViewBag.SoftwareTypeName = viewModel.SoftwareType.Name;
                
                return View(viewModel);
            }
            return new BadRequestResult();            
        }

        // GET: SoftwareTypes/Create
        public IActionResult Create(int projectId)
        {
            if (projectId != null)
            {
                ViewBag.ProjectId = projectId;
                return View();
            }
            return new BadRequestResult();
        }

        // POST: SoftwareTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Date,Description,ProjectId")] SoftwareType softwareType)
        {
            softwareType.CreationDate = DateTime.Now;
            softwareType.LastModificationDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(softwareType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(softwareType);
        }

        // GET: SoftwareTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareType = await _context.SoftwareType.FindAsync(id);
            if (softwareType == null)
            {
                return NotFound();
            }
            return View(softwareType);
        }

        // POST: SoftwareTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Date,Description")] SoftwareType softwareType)
        {
            if (id != softwareType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(softwareType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoftwareTypeExists(softwareType.Id))
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
            return View(softwareType);
        }

        // GET: SoftwareTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareType = await _context.SoftwareType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (softwareType == null)
            {
                return NotFound();
            }

            return View(softwareType);
        }

        // POST: SoftwareTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var softwareType = await _context.SoftwareType.FindAsync(id);
            _context.SoftwareType.Remove(softwareType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SoftwareTypeExists(int id)
        {
            return _context.SoftwareType.Any(e => e.Id == id);
        }
    }
}
