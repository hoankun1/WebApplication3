using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication3.Data;
using WebApplication3.Models;
using WebApplication3.Repositories;

namespace WebApplication3.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public ProductController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var products = await _productRepository.GetAllAsync();

            if (categoryId.HasValue)
            {
                products = products
                    .Where(p => p.CategoryId == categoryId.Value)
                    .ToList();
            }

            ViewBag.Categories =
                await _categoryRepository.GetAllAsync();

            ViewBag.SelectedCategory = categoryId;

            return View(products);
        }

        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = new SelectList(
                await _categoryRepository.GetAllAsync(),
                "Id",
                "Name");

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            Product product,
            IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    await _categoryRepository.GetAllAsync(),
                    "Id",
                    "Name");

                return View(product);
            }

            if (imageFile != null)
            {
                string uploadsFolder =
                    Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images");

                Directory.CreateDirectory(uploadsFolder);

                string fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(imageFile.FileName);

                string filePath =
                    Path.Combine(uploadsFolder, fileName);

                using var stream =
                    new FileStream(filePath, FileMode.Create);

                await imageFile.CopyToAsync(stream);

                product.ImageUrl = "/images/" + fileName;
            }

            await _productRepository.AddAsync(product);

            TempData["Success"] = "Thêm sản phẩm thành công";

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Update(int id)
        {
            var product =
                await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            ViewBag.Categories = new SelectList(
                await _categoryRepository.GetAllAsync(),
                "Id",
                "Name",
                product.CategoryId);

            return View(product);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            int id,
            Product product,
            IFormFile? imageFile)
        {
            if (id != product.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    await _categoryRepository.GetAllAsync(),
                    "Id",
                    "Name",
                    product.CategoryId);

                return View(product);
            }

            if (imageFile != null)
            {
                string uploadsFolder =
                    Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images");

                Directory.CreateDirectory(uploadsFolder);

                string fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(imageFile.FileName);

                string filePath =
                    Path.Combine(uploadsFolder, fileName);

                using var stream =
                    new FileStream(filePath, FileMode.Create);

                await imageFile.CopyToAsync(stream);

                product.ImageUrl = "/images/" + fileName;
            }

            await _productRepository.UpdateAsync(product);

            TempData["Success"] = "Cập nhật sản phẩm thành công";

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product =
                await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            var cartItem =
                _context.CartItems
                .FirstOrDefault(x => x.ProductId == id);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = id,
                    Quantity = 1
                };

                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Đã thêm vào giỏ hàng";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product =
                await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);

            TempData["Success"] = "Xóa sản phẩm thành công";

            return RedirectToAction(nameof(Index));
        }
    }
}