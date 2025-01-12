using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Models;
using FurnitureShop.Utils;

namespace FurnitureShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly FurnitureShopContext _context;

        public UserController(FurnitureShopContext context)
        {
            _context = context;
        }

        // GET: Admin/User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Admin/User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] User user)
        {
            if (ModelState.IsValid)
            {
                var adminuser = HttpContext.Session.Get<AdminUser>("adminInfo");
                if (adminuser != null)
                {
                    user.CreatedBy = adminuser.Name;
                    user.CreatedDate = DateTime.Now;
                    user.UpdatedBy = adminuser.Name;
                    user.UpdatedDate = DateTime.Now;
                }
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    existingUser.Name = user.Name;
                    existingUser.Address = user.Address;
                    existingUser.Phone = user.Phone;
                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                    var adminuser = HttpContext.Session.Get<AdminUser>("adminInfo");
                    if (adminuser != null)
                    {
                        existingUser.CreatedBy = adminuser.Name;
                        existingUser.UpdatedBy = adminuser.Name;
                        existingUser.UpdatedDate = DateTime.Now;
                    }

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Admin/User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                var hasOrders = await _context.Orders.AnyAsync(o => o.User_id == id);
                if (hasOrders)
                {
                    TempData["ErrorMessage"] = "Cannot delete user. The user has associated orders.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
