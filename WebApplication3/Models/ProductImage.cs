using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; } = string.Empty;

        public int ProductId { get; set; }

        public Product? Product { get; set; }
    }
}