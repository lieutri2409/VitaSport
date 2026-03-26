using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Helpers;
using Shopping.Models.ViewModels;
using System.Security.Claims;

namespace Shopping.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        const string CART_KEY = "UserCart";

        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY) ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var addresses = _context.Addresses
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .ToList();

            var defaultAddress = addresses.FirstOrDefault(x => x.IsDefault);

            var model = new CheckoutViewModel
            {
                CartItems = cart,
                SavedAddresses = addresses,
                SelectedAddressId = defaultAddress?.Id,
                UseNewAddress = !addresses.Any(),
                PaymentMethod = "COD"
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ApplyCoupon(string code)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY) ?? new List<CartItem>();

            if (!cart.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng đang trống." });
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });
            }

            decimal subTotal = cart.Sum(x => x.Price * x.Quantity);
            decimal discountAmount = 0;

            var coupon = _context.Coupons.FirstOrDefault(x =>
                x.Code == code.Trim().ToUpper() &&
                x.IsActive &&
                x.StartDate.Date <= DateTime.Today &&
                x.EndDate.Date >= DateTime.Today &&
                x.UsedCount < x.UsageLimit);

            if (coupon == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn." });
            }

            if (subTotal < coupon.MinimumOrderAmount)
            {
                return Json(new
                {
                    success = false,
                    message = $"Đơn hàng phải từ {coupon.MinimumOrderAmount:N0} đ để áp dụng mã này."
                });
            }

            if (coupon.IsPercentage)
            {
                discountAmount = subTotal * coupon.DiscountValue / 100m;

                if (coupon.MaxDiscountAmount.HasValue && discountAmount > coupon.MaxDiscountAmount.Value)
                {
                    discountAmount = coupon.MaxDiscountAmount.Value;
                }
            }
            else
            {
                discountAmount = coupon.DiscountValue;
            }

            if (discountAmount > subTotal)
            {
                discountAmount = subTotal;
            }

            var finalTotal = subTotal - discountAmount;

            return Json(new
            {
                success = true,
                message = "Áp dụng mã thành công.",
                discountAmount = discountAmount,
                finalTotal = finalTotal
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY) ?? new List<CartItem>();

            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.GetUserAsync(User);

            if (user == null || string.IsNullOrEmpty(userId))
                return NotFound();

            var addresses = _context.Addresses
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .ToList();

            model.CartItems = cart;
            model.SavedAddresses = addresses;
            model.PaymentMethod = "COD";

            string receiverName;
            string receiverPhone;
            string shippingAddress;

            if (!model.UseNewAddress && model.SelectedAddressId.HasValue)
            {
                var selectedAddress = addresses.FirstOrDefault(x => x.Id == model.SelectedAddressId.Value);

                if (selectedAddress == null)
                {
                    TempData["Error"] = "Địa chỉ đã chọn không hợp lệ.";
                    return View("Checkout", model);
                }

                receiverName = selectedAddress.FullName;
                receiverPhone = selectedAddress.PhoneNumber;
                shippingAddress = selectedAddress.FullAddress;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.FullName))
                    ModelState.AddModelError("FullName", "Vui lòng nhập tên người nhận.");

                if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                    ModelState.AddModelError("PhoneNumber", "Vui lòng nhập số điện thoại.");
                else if (!System.Text.RegularExpressions.Regex.IsMatch(model.PhoneNumber, @"^0\d{9}$"))
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0.");

                if (string.IsNullOrWhiteSpace(model.StreetAddress))
                    ModelState.AddModelError("StreetAddress", "Vui lòng nhập địa chỉ chi tiết.");

                if (string.IsNullOrWhiteSpace(model.Ward))
                    ModelState.AddModelError("Ward", "Vui lòng nhập phường/xã.");

                if (string.IsNullOrWhiteSpace(model.District))
                    ModelState.AddModelError("District", "Vui lòng nhập quận/huyện.");

                if (string.IsNullOrWhiteSpace(model.City))
                    ModelState.AddModelError("City", "Vui lòng nhập tỉnh/thành phố.");

                if (!ModelState.IsValid)
                {
                    model.SavedAddresses = addresses;
                    model.CartItems = cart;
                    model.UseNewAddress = true;
                    return View("Checkout", model);
                }

                receiverName = model.FullName!;
                receiverPhone = model.PhoneNumber!;
                shippingAddress = $"{model.StreetAddress}, {model.Ward}, {model.District}, {model.City}";
            }

            foreach (var item in cart)
            {
                var productCheck = _context.Products.Find(item.ProductId);
                if (productCheck == null)
                    continue;

                if (productCheck.Quantity < item.Quantity)
                {
                    TempData["Error"] = $"Sản phẩm {productCheck.Name} không đủ hàng.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            decimal subTotal = cart.Sum(x => x.Price * x.Quantity);
            decimal discountAmount = 0;
            string? appliedCouponCode = null;

            if (!string.IsNullOrWhiteSpace(model.CouponCode))
            {
                var code = model.CouponCode.Trim().ToUpper();

                var coupon = _context.Coupons.FirstOrDefault(x =>
                    x.Code == code &&
                    x.IsActive &&
                    x.StartDate.Date <= DateTime.Today &&
                    x.EndDate.Date >= DateTime.Today &&
                    x.UsedCount < x.UsageLimit);

                if (coupon == null)
                {
                    ModelState.AddModelError("CouponCode", "Mã giảm giá không hợp lệ hoặc đã hết hạn.");
                }
                else if (subTotal < coupon.MinimumOrderAmount)
                {
                    ModelState.AddModelError("CouponCode", $"Đơn hàng phải từ {coupon.MinimumOrderAmount:N0} đ để áp dụng mã này.");
                }
                else
                {
                    if (coupon.IsPercentage)
                    {
                        discountAmount = subTotal * coupon.DiscountValue / 100m;

                        if (coupon.MaxDiscountAmount.HasValue && discountAmount > coupon.MaxDiscountAmount.Value)
                        {
                            discountAmount = coupon.MaxDiscountAmount.Value;
                        }
                    }
                    else
                    {
                        discountAmount = coupon.DiscountValue;
                    }

                    if (discountAmount > subTotal)
                    {
                        discountAmount = subTotal;
                    }

                    appliedCouponCode = coupon.Code;
                }
            }

            if (!ModelState.IsValid)
            {
                model.SavedAddresses = addresses;
                model.CartItems = cart;
                model.DiscountAmount = discountAmount;
                return View("Checkout", model);
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalPrice = (double)(subTotal - discountAmount),
                Status = "Pending",
                ReceiverName = receiverName,
                ReceiverPhone = receiverPhone,
                ShippingAddress = shippingAddress,
                CouponCode = appliedCouponCode,
                DiscountAmount = (double)discountAmount,
                PaymentMethod = "COD"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                var product = _context.Products.Find(item.ProductId);

                if (product == null)
                    continue;

                product.Quantity -= item.Quantity;

                var detail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };

                _context.OrderDetails.Add(detail);
            }

            if (!string.IsNullOrWhiteSpace(appliedCouponCode))
            {
                var usedCoupon = _context.Coupons.FirstOrDefault(x => x.Code == appliedCouponCode);
                if (usedCoupon != null)
                {
                    usedCoupon.UsedCount += 1;
                }
            }

            await _context.SaveChangesAsync();

            HttpContext.Session.Remove(CART_KEY);

            return RedirectToAction("Success", new { id = order.Id });
        }

        public async Task<IActionResult> Success(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == order.UserId);

            ViewBag.Order = order;
            ViewBag.User = user;

            return View();
        }

        public async Task<IActionResult> MyOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> Manage(string status)
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.Status == status);
            }

            var result = await orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(result);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!User.IsInRole("Admin") && !User.IsInRole("NhanVien"))
            {
                if (order.UserId != currentUserId)
                {
                    return Forbid();
                }
            }

            var orderDetails = await _context.OrderDetails
                .Include(o => o.Product)
                .Where(o => o.OrderId == id)
                .ToListAsync();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == order.UserId);

            ViewBag.Order = order;
            ViewBag.User = user;

            return View(orderDetails);
        }

        [Authorize(Roles = "Admin,NhanVien")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();

            order.Status = status;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = id });
        }
    }
}