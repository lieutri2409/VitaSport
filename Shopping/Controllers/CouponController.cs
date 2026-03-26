using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Models.ViewModels;

namespace Shopping.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly AppDbContext _context;

        public CouponController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var coupons = _context.Coupons
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(coupons);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CouponViewModel
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7),
                UsageLimit = 100,
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CouponViewModel model)
        {
            if (_context.Coupons.Any(x => x.Code == model.Code))
            {
                ModelState.AddModelError("Code", "Mã giảm giá đã tồn tại.");
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var coupon = new Coupon
            {
                Code = model.Code.Trim().ToUpper(),
                Name = model.Name,
                Description = model.Description,
                IsPercentage = model.IsPercentage,
                DiscountValue = model.DiscountValue,
                MaxDiscountAmount = model.MaxDiscountAmount,
                MinimumOrderAmount = model.MinimumOrderAmount,
                UsageLimit = model.UsageLimit,
                UsedCount = 0,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.Coupons.Add(coupon);
            _context.SaveChanges();

            TempData["Success"] = "Tạo mã giảm giá thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var coupon = _context.Coupons.Find(id);
            if (coupon == null) return NotFound();

            var model = new CouponViewModel
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Name = coupon.Name,
                Description = coupon.Description,
                IsPercentage = coupon.IsPercentage,
                DiscountValue = coupon.DiscountValue,
                MaxDiscountAmount = coupon.MaxDiscountAmount,
                MinimumOrderAmount = coupon.MinimumOrderAmount,
                UsageLimit = coupon.UsageLimit,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                IsActive = coupon.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CouponViewModel model)
        {
            var coupon = _context.Coupons.Find(model.Id);
            if (coupon == null) return NotFound();

            if (_context.Coupons.Any(x => x.Code == model.Code && x.Id != model.Id))
            {
                ModelState.AddModelError("Code", "Mã giảm giá đã tồn tại.");
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            coupon.Code = model.Code.Trim().ToUpper();
            coupon.Name = model.Name;
            coupon.Description = model.Description;
            coupon.IsPercentage = model.IsPercentage;
            coupon.DiscountValue = model.DiscountValue;
            coupon.MaxDiscountAmount = model.MaxDiscountAmount;
            coupon.MinimumOrderAmount = model.MinimumOrderAmount;
            coupon.UsageLimit = model.UsageLimit;
            coupon.StartDate = model.StartDate;
            coupon.EndDate = model.EndDate;
            coupon.IsActive = model.IsActive;

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật mã giảm giá thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Toggle(int id)
        {
            var coupon = _context.Coupons.Find(id);
            if (coupon == null) return NotFound();

            coupon.IsActive = !coupon.IsActive;
            _context.SaveChanges();

            TempData["Success"] = "Đã cập nhật trạng thái mã giảm giá.";
            return RedirectToAction(nameof(Index));
        }
    }
}