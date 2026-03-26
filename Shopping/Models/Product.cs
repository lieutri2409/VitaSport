using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Shopping.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tên sản phẩm")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Bạn chưa nhập mô tả")]
        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string Description { get; set; } = default!;

        [Required(ErrorMessage = "Bạn chưa nhập giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập số lượng")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
        public int Quantity { get; set; }

        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Bạn chưa chọn danh mục")]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = default!;
    }
}