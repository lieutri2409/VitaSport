using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Shopping.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> ListPublic(
    int? categoryId,
    string? keyword,
    decimal? minPrice,
    decimal? maxPrice,
    string? sortBy,
    string? stockStatus,
    int page = 1,
    bool ajax = false)
        {
            int pageSize = 8;

            var publicCategories = await _context.Categories.ToListAsync();
            publicCategories.Insert(0, new Category { CategoryId = 0, CategoryName = "Tất cả sản phẩm" });
            ViewBag.PublicCategoryList = new SelectList(publicCategories, "CategoryId", "CategoryName", categoryId);

            var productsQuery = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(keyword));
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrWhiteSpace(stockStatus))
            {
                if (stockStatus == "inStock")
                {
                    productsQuery = productsQuery.Where(p => p.Quantity > 0);
                }
                else if (stockStatus == "outStock")
                {
                    productsQuery = productsQuery.Where(p => p.Quantity <= 0);
                }
            }

            productsQuery = sortBy switch
            {
                "priceAsc" => productsQuery.OrderBy(p => p.Price),
                "priceDesc" => productsQuery.OrderByDescending(p => p.Price),
                "nameAsc" => productsQuery.OrderBy(p => p.Name),
                "nameDesc" => productsQuery.OrderByDescending(p => p.Name),
                _ => productsQuery.OrderByDescending(p => p.Id)
            };

            int totalProducts = await productsQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            if (page < 1) page = 1;
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var products = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                var currentCategory = await _context.Categories.FindAsync(categoryId.Value);
                if (currentCategory != null)
                {
                    ViewData["CurrentCategoryName"] = currentCategory.CategoryName;
                }
            }

            ViewData["CurrentCategoryId"] = categoryId;
            ViewData["ProductCount"] = totalProducts;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            ViewData["Keyword"] = keyword;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["SortBy"] = sortBy;
            ViewData["StockStatus"] = stockStatus;

            if (ajax)
            {
                return PartialView("_ProductListPartial", products);
            }

            return View("Index", products);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Manage(int? categoryId)
        {
            var manageCategories = await _context.Categories.ToListAsync();
            manageCategories.Insert(0, new Category { CategoryId = 0, CategoryName = "Tất cả sản phẩm" });
            ViewBag.ManageCategoryList = new SelectList(manageCategories, "CategoryId", "CategoryName", categoryId);

            var productsQuery = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await productsQuery.ToListAsync();

            ViewData["ShowAddProductButton"] = true;

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                var currentCategory = await _context.Categories.FindAsync(categoryId.Value);
                if (currentCategory != null)
                {
                    ViewData["CurrentCategoryName"] = currentCategory.CategoryName;
                }
            }

            ViewData["ProductCount"] = products.Count;

            return View("Manage", products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Product model)
        {
            ModelState.Remove("Category");
            ModelState.Remove("Category.CategoryName");

            if (model.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Bạn chưa chọn ảnh sản phẩm");
            }

            if (ModelState.IsValid)
            {
                model.ImageUrl = await SaveImageAsync(model.ImageFile!);

                _context.Products.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }

            var currentCategories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(currentCategories, "CategoryId", "CategoryName", model.CategoryId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Product model)
        {
            ModelState.Remove("Category");
            ModelState.Remove("Category.CategoryName");

            var product = await _context.Products.FindAsync(model.Id);
            if (product == null) return NotFound();

            if (ModelState.IsValid)
            {
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Quantity = model.Quantity;
                product.CategoryId = model.CategoryId;

                if (model.ImageFile != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        DeleteImage(product.ImageUrl);
                    }

                    product.ImageUrl = await SaveImageAsync(model.ImageFile);
                }

                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage));
            }

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", model.CategoryId);

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    DeleteImage(product.ImageUrl);
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Manage));
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/images/products/" + uniqueFileName;
        }

        private void DeleteImage(string imageUrl)
        {
            string fileName = Path.GetFileName(imageUrl);
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}