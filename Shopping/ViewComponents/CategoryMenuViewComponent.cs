using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;

namespace Shopping.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public CategoryMenuViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách loại hàng từ database
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
    }
}