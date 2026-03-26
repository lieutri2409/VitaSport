using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;

[Authorize(Roles = "Admin")]
public class AdminOrderController : Controller
{
    private readonly AppDbContext _context;

    public AdminOrderController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var orders = _context.Orders
            .OrderByDescending(x => x.OrderDate)
            .ToList();

        return View(orders);
    }
}