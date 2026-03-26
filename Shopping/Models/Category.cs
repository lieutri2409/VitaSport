using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tên danh mục")]
        [StringLength(50, ErrorMessage = "Tên danh mục tối đa 50 ký tự")]
        public string CategoryName { get; set; } = default!;
        public virtual ICollection<Product>? Products { get; set; }
    }
}