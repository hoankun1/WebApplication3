using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cartItems = await _context.CartItems
                .Include(x => x.Product)
                .ToListAsync();

            ViewBag.Total =
                cartItems.Sum(x =>
                    (x.Product?.Price ?? 0) * x.Quantity);

            return View(cartItems);
        }

        public async Task<IActionResult> Increase(int id)
        {
            var item =
                await _context.CartItems.FindAsync(id);

            if (item != null)
            {
                item.Quantity++;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Decrease(int id)
        {
            var item =
                await _context.CartItems.FindAsync(id);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int id)
        {
            var item =
                await _context.CartItems.FindAsync(id);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(
            string FullName,
            string PhoneNumber,
            string Province,
            string District,
            string Ward,
            string AddressDetail,
            string PaymentMethod)
        {
            var cartItems = await _context.CartItems
                .Include(x => x.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng đang trống";
                return RedirectToAction(nameof(Index));
            }

            var order = new Order
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
                FullName = FullName,
                PhoneNumber = PhoneNumber,
                Address =
                    $"{AddressDetail}, {Ward}, {District}, {Province}",
                TotalAmount = cartItems.Sum(x =>
                    (x.Product?.Price ?? 0) * x.Quantity),
                Status = "Confirmed"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var detail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product?.Price ?? 0
                };

                _context.OrderDetails.Add(detail);
            }

            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Đặt hàng thành công. Mã đơn hàng: #{order.Id}";

            return RedirectToAction(
                "Detail",
                "Order",
                new { id = order.Id });
        }
    }
}