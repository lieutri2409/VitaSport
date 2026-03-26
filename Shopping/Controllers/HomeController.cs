using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;

namespace Shopping.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // NOTE: Lấy 12 sản phẩm mới nhất để hiển thị ở slider sản phẩm nổi bật
            var featuredProducts = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .Take(12)
                .ToListAsync();

            // NOTE: Lấy các mã giảm giá đang còn hiệu lực để hiển thị ở homepage
            var hotCoupons = await _context.Coupons
                .Where(c => c.IsActive
                            && c.StartDate <= DateTime.Now
                            && c.EndDate >= DateTime.Now
                            && c.UsedCount < c.UsageLimit)
                .OrderByDescending(c => c.DiscountValue)
                .Take(3)
                .ToListAsync();

            // NOTE: Lấy riêng sản phẩm theo từng danh mục để tăng tính trực quan ở homepage
            var shoeProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 4)
                .OrderByDescending(p => p.Id)
                .Take(4)
                .ToListAsync();

            var gymProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 7)
                .OrderByDescending(p => p.Id)
                .Take(4)
                .ToListAsync();

            var ballProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == 8)
                .OrderByDescending(p => p.Id)
                .Take(4)
                .ToListAsync();

            ViewBag.HotCoupons = hotCoupons;
            ViewBag.ShoeProducts = shoeProducts;
            ViewBag.GymProducts = gymProducts;
            ViewBag.BallProducts = ballProducts;

            return View(featuredProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}