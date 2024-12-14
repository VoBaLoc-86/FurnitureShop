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
    public class SettingController : Controller
    {
        private readonly FurnitureShopContext _context;

        public SettingController(FurnitureShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Setting
        public async Task<IActionResult> Index()
        {
            return View(await _context.Settings.ToListAsync());
        }

        // GET: Admin/Setting/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.SET_ID == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // GET: Admin/Setting/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Setting/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SET_ID,Name,Value,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Setting setting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(setting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(setting);
        }

        // GET: Admin/Setting/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings.FindAsync(id);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }

        // POST: Admin/Setting/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SET_ID,Name,Value,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Setting setting)
        {
            if (id != setting.SET_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(setting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SettingExists(setting.SET_ID))
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
            return View(setting);
        }

        // GET: Admin/Setting/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.SET_ID == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // POST: Admin/Setting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var setting = await _context.Settings.FindAsync(id);
            if (setting != null)
            {
                _context.Settings.Remove(setting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.SET_ID == id);
        }
    }
}
