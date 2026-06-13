using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";

        [Required]
        public string FullName { get; set; } = "";

        [Required]
        public string PhoneNumber { get; set; } = "";

        [Required]
        public string Address { get; set; } = "";

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Confirmed";

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public ICollection<OrderDetail> OrderDetails { get; set; }
            = new List<OrderDetail>();
    }
}