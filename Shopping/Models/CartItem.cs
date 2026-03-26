namespace Shopping.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Total => Price * Quantity;
    }
}