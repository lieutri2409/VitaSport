using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string? UserId { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        [StringLength(100)]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0")]
        public string PhoneNumber { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ chi tiết")]
        [StringLength(200)]
        public string StreetAddress { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập phường/xã")]
        [StringLength(100)]
        public string Ward { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập quận/huyện")]
        [StringLength(100)]
        public string District { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập tỉnh/thành phố")]
        [StringLength(100)]
        public string City { get; set; } = default!;

        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string FullAddress => $"{StreetAddress}, {Ward}, {District}, {City}";
    }
}