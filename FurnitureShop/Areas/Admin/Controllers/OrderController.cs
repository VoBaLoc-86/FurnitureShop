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
    public class OrderController : Controller
    {
        private readonly FurnitureShopContext _context;

        public OrderController(FurnitureShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Order
        
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .ToListAsync();
            return View(orders);
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Order_Details)  // Chú ý là `Order_Details` với dấu gạch dưới
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }



        // GET: Admin/Order/Create
        public IActionResult Create()
        {
            ViewData["User_id"] = new SelectList(_context.Users, "Id", "Address");
            return View();
        }

        // POST: Admin/Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,User_id,Total_price,Status,Shipping_address,Payment_status,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["User_id"] = new SelectList(_context.Users, "Id", "Address", order.User_id);
            return View(order);
        }




        // GET: Admin/Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["User_id"] = new SelectList(_context.Users, "Id", "Address", order.User_id);
            return View(order);
        }

        // POST: Admin/Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,User_id,Total_price,Status,Shipping_address,Payment_status,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["User_id"] = new SelectList(_context.Users, "Id", "Address", order.User_id);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status, string paymentStatus)
        {
            var order = await _context.Orders
                .Include(o => o.Order_Details) // Bao gồm Order_Details
                .ThenInclude(od => od.Product)
                .Include(o => o.User) // Bao gồm User nếu cần
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var originalStatus = order.Status;

            // Nếu đơn hàng đang ở trạng thái "Processing" và trạng thái mới là khác "Processing"
            if (originalStatus == "Processing" && status != "Processing")
            {
                foreach (var orderDetail in order.Order_Details)
                {
                    var product = await _context.Products.FindAsync(orderDetail.Product_id);

                    if (product != null)
                    {
                        // Hoàn lại stock nếu trạng thái chuyển từ "Processing" sang "Cancelled"
                        if (status == "Cancelled")
                        {
                            product.Stock += orderDetail.Quantity;
                        }

                        // Cập nhật sản phẩm
                        _context.Update(product);
                    }
                }
            }

            // Nếu đơn hàng chuyển từ trạng thái khác sang "Processing"
            if (status == "Processing" && originalStatus != "Processing")
            {
                foreach (var orderDetail in order.Order_Details)
                {
                    var product = await _context.Products.FindAsync(orderDetail.Product_id);

                    if (product != null)
                    {
                        // Trừ số lượng sản phẩm khi chuyển sang "Processing"
                        product.Stock -= orderDetail.Quantity;

                        // Đảm bảo số lượng sản phẩm không âm
                        if (product.Stock < 0)
                        {
                            product.Stock = 0;
                        }

                        // Cập nhật sản phẩm
                        _context.Update(product);
                    }
                }
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = status;
            order.Payment_status = paymentStatus;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(Details), new { id = order.Id });
        }



        // GET: Admin/Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.Include(o => o.Order_Details)
                                    .FirstOrDefaultAsync(m => m.Id == id);
            if (order != null)
            {
                _context.OrderDetails.RemoveRange(order.Order_Details);
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        
    }
}
