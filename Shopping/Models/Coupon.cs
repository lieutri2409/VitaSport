using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        [StringLength(50)]
        public string Code { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập tên mã")]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(300)]
        public string? Description { get; set; }

        public bool IsPercentage { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm không hợp lệ")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxDiscountAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MinimumOrderAmount { get; set; }

        public int UsageLimit { get; set; }

        public int UsedCount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}