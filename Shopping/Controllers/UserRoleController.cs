using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Models.ViewModels;

[Authorize(Roles = "Admin")]
public class UserRoleController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRoleController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // =========================
    // Danh sách user
    // =========================
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();

        var model = new List<UserRoleViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            model.Add(new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "KhachHang"
            });
        }

        return View(model);
    }

    // =========================
    // Cập nhật Role
    // =========================
    [HttpPost]
    public async Task<IActionResult> UpdateRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, role);
        }

        return RedirectToAction("Index");
    }

    // =========================
    // Xóa user
    // =========================
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        return RedirectToAction("Index");
    }
}