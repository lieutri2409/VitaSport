using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;

        public DateTime OrderDate { get; set; }

        public double TotalPrice { get; set; }

        public string Status { get; set; } = default!;

        [StringLength(100)]
        public string? ReceiverName { get; set; }

        [StringLength(20)]
        public string? ReceiverPhone { get; set; }

        [StringLength(300)]
        public string? ShippingAddress { get; set; }

        public ApplicationUser User { get; set; } = default!;

        public List<OrderDetail> OrderDetails { get; set; } = new();

        [StringLength(50)]
        public string? CouponCode { get; set; }

        public double DiscountAmount { get; set; }

        public string PaymentMethod { get; set; } = "COD";
    }
}