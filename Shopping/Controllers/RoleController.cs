using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Models.ViewModels;

[Authorize(Roles = "Admin")]
public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleController(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // Danh sách role + thống kê user
    public async Task<IActionResult> Index()
    {
        var roles = _roleManager.Roles.ToList();

        var model = new List<RoleViewModel>();

        foreach (var role in roles)
        {
            var users = await _userManager.GetUsersInRoleAsync(role.Name);

            model.Add(new RoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                UserCount = users.Count
            });
        }

        return View(model);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string roleName)
    {
        if (!string.IsNullOrEmpty(roleName))
        {
            var exist = await _roleManager.RoleExistsAsync(roleName);

            if (!exist)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role != null)
        {
            await _roleManager.DeleteAsync(role);
        }

        return RedirectToAction("Index");
    }
}