using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(0.01, 1000000000)]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        // Ảnh chính
        public string? ImageUrl { get; set; }

        // Khóa ngoại Category
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        // Navigation Property
        public Category? Category { get; set; }

        // Danh sách ảnh phụ
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}