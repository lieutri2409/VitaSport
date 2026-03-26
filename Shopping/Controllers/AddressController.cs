using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;

namespace Shopping.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddressController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var addresses = _context.Addresses
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .ToList();

            return View(addresses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Address model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            model.UserId = user.Id;

            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

                var addresses = _context.Addresses
                    .Where(x => x.UserId == user.Id)
                    .OrderByDescending(x => x.IsDefault)
                    .ThenByDescending(x => x.CreatedAt)
                    .ToList();

                return View("Index", addresses);
            }

            bool hasAnyAddress = _context.Addresses.Any(x => x.UserId == user.Id);

            if (model.IsDefault)
            {
                var oldDefaultAddresses = _context.Addresses.Where(x => x.UserId == user.Id).ToList();
                foreach (var item in oldDefaultAddresses)
                {
                    item.IsDefault = false;
                }
            }
            else if (!hasAnyAddress)
            {
                model.IsDefault = true;
            }

            _context.Addresses.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm địa chỉ thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var address = _context.Addresses.FirstOrDefault(x => x.Id == id && x.UserId == user.Id);
            if (address == null) return NotFound();

            var allAddresses = _context.Addresses.Where(x => x.UserId == user.Id).ToList();
            foreach (var item in allAddresses)
            {
                item.IsDefault = false;
            }

            address.IsDefault = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã đặt địa chỉ mặc định.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var address = _context.Addresses.FirstOrDefault(x => x.Id == id && x.UserId == user.Id);
            if (address == null) return NotFound();

            bool wasDefault = address.IsDefault;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            if (wasDefault)
            {
                var nextAddress = _context.Addresses
                    .Where(x => x.UserId == user.Id)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefault();

                if (nextAddress != null)
                {
                    nextAddress.IsDefault = true;
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Success"] = "Đã xóa địa chỉ.";
            return RedirectToAction(nameof(Index));
        }
    }
}