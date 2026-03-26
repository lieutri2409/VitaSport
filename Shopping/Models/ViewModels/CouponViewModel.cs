using System.ComponentModel.DataAnnotations;

namespace Shopping.Models.ViewModels
{
    public class CouponViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        public string Code { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập tên mã")]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public bool IsPercentage { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm")]
        public decimal DiscountValue { get; set; }

        public decimal? MaxDiscountAmount { get; set; }

        public decimal MinimumOrderAmount { get; set; }

        public int UsageLimit { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}