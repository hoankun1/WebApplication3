using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // HISTORY
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.Orders
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // DETAIL
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // REORDER
        public async Task<IActionResult> ReOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(x => x.OrderDetails)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (order == null)
                return NotFound();

            foreach (var item in order.OrderDetails)
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(x =>
                        x.ProductId == item.ProductId);

                if (cartItem == null)
                {
                    _context.CartItems.Add(new CartItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }
                else
                {
                    cartItem.Quantity += item.Quantity;
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã thêm lại sản phẩm vào giỏ hàng";

            return RedirectToAction("Index", "Cart");
        }
    }
}