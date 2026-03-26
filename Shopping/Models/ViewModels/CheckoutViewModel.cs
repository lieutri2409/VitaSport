using System.ComponentModel.DataAnnotations;

namespace Shopping.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new();
        public List<Address> SavedAddresses { get; set; } = new();

        public int? SelectedAddressId { get; set; }
        public bool UseNewAddress { get; set; }

        [Display(Name = "Tên người nhận")]
        public string? FullName { get; set; }

        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ chi tiết")]
        public string? StreetAddress { get; set; }

        [Display(Name = "Phường/Xã")]
        public string? Ward { get; set; }

        [Display(Name = "Quận/Huyện")]
        public string? District { get; set; }

        [Display(Name = "Tỉnh/Thành phố")]
        public string? City { get; set; }

        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }

        public string PaymentMethod { get; set; } = "COD";

        public decimal SubTotal => CartItems.Sum(x => x.Price * x.Quantity);
        public decimal FinalTotal => SubTotal - DiscountAmount;
    }
}