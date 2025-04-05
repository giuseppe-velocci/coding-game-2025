using System.ComponentModel.DataAnnotations;

namespace OrderService.Orders
{
    public class Address
    {
        [Key]
        public long AddressId { get; set; }

        [MaxLength(200)]
        public string Street { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(20)]
        public string ZipCode { get; set; }
    }
}
