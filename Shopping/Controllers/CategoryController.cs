using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore; 

namespace Shopping.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // 1. HIỂN THỊ DANH SÁCH LOẠI SẢN PHẨM
        [HttpGet]
        public IActionResult Index()
        {
            // Thêm .Include(c => c.Products) để nạp dữ liệu sản phẩm từ DB vào bộ nhớ
            var categories = _context.Categories
                                     .Include(c => c.Products)
                                     .ToList();
            return View(categories);
        }

        // 2. THÊM MỚI LOẠI SẢN PHẨM (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. THÊM MỚI LOẠI SẢN PHẨM (POST)
        [HttpPost]
        public IActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // 4. CHỈNH SỬA LOẠI SẢN PHẨM (GET)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // 5. CHỈNH SỬA LOẠI SẢN PHẨM (POST)
        [HttpPost]
        public IActionResult Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // 6. XÓA LOẠI SẢN PHẨM (GET)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // 7. XÓA LOẠI SẢN PHẨM (POST)
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}