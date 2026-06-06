using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication3.Models;
using WebApplication3.Repositories;

namespace WebApplication3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _productRepository.GetAllAsync());
        }

        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = new SelectList(
                await _categoryRepository.GetAllAsync(),
                "Id",
                "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    await _categoryRepository.GetAllAsync(),
                    "Id",
                    "Name");

                return View(product);
            }

            await _productRepository.AddAsync(product);

            return RedirectToAction(nameof(Index));
        }

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
        public async Task<IActionResult> Update(
            int id,
            Product product)
        {
            if (id != product.Id)
                return NotFound();

            await _productRepository.UpdateAsync(product);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product =
                await _productRepository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}