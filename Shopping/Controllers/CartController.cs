using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Helpers;

namespace Shopping.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private const string CART_KEY = "UserCart";

        public CartController(AppDbContext context) => _context = context;

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            return View(cart);
        }

        public IActionResult AddToCart(int id)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["RequireLoginMessage"] = "Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng và đặt hàng.";
                return RedirectToAction("Index", "Home");
            }

            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(p => p.ProductId == id);

            if (product.Quantity <= 0)
            {
                TempData["Error"] = "Sản phẩm đã hết hàng";
                return RedirectToAction("ListPublic", "Product");
            }

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }

            HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
            return RedirectToAction("Index");
        }

        public IActionResult Increase(int id)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["RequireLoginMessage"] = "Bạn cần đăng nhập để thao tác với giỏ hàng.";
                return RedirectToAction("Index", "Home");
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY);
            if (cart != null)
            {
                var item = cart.FirstOrDefault(p => p.ProductId == id);
                if (item != null)
                {
                    item.Quantity++;
                }
                HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Decrease(int id)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["RequireLoginMessage"] = "Bạn cần đăng nhập để thao tác với giỏ hàng.";
                return RedirectToAction("Index", "Home");
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY);
            if (cart != null)
            {
                var item = cart.FirstOrDefault(p => p.ProductId == id);
                if (item != null)
                {
                    if (item.Quantity > 1)
                        item.Quantity--;
                    else
                        cart.Remove(item);
                }
                HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["RequireLoginMessage"] = "Bạn cần đăng nhập để thao tác với giỏ hàng.";
                return RedirectToAction("Index", "Home");
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY);
            if (cart != null)
            {
                cart.RemoveAll(p => p.ProductId == id);
                HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }
    }
}