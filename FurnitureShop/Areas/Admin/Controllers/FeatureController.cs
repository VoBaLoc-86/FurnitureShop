using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Models;

namespace FurnitureShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeatureController : Controller
    {
        private readonly FurnitureShopContext _context;

        public FeatureController(FurnitureShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Feature
        public async Task<IActionResult> Index()
        {
            return View(await _context.Features.ToListAsync());
        }

        // GET: Admin/Feature/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Features
                .FirstOrDefaultAsync(m => m.FEA_ID == id);
            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        // GET: Admin/Feature/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Feature/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FEA_ID,Icon,Title,Subtitle,DisplayOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Feature feature)
        {
            if (ModelState.IsValid)
            {
                _context.Add(feature);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feature);
        }

        // GET: Admin/Feature/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Features.FindAsync(id);
            if (feature == null)
            {
                return NotFound();
            }
            return View(feature);
        }

        // POST: Admin/Feature/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FEA_ID,Icon,Title,Subtitle,DisplayOrder,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Feature feature)
        {
            if (id != feature.FEA_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeatureExists(feature.FEA_ID))
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
            return View(feature);
        }

        // GET: Admin/Feature/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Features
                .FirstOrDefaultAsync(m => m.FEA_ID == id);
            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        // POST: Admin/Feature/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feature = await _context.Features.FindAsync(id);
            if (feature != null)
            {
                _context.Features.Remove(feature);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeatureExists(int id)
        {
            return _context.Features.Any(e => e.FEA_ID == id);
        }
    }
}
