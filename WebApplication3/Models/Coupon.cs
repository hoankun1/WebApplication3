using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = "";

        public decimal DiscountPercent { get; set; }
    }
}