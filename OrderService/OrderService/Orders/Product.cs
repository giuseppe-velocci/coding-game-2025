using System.ComponentModel.DataAnnotations;

namespace OrderService.Orders
{
    public class Product
    {
        [Key]
        public long ProductId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; } // to invalid a product that is removed from Product service
    }
}
