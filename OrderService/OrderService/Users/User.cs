using OrderService.Orders;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderService.Users
{
    public class User
    {
        [Key]
        public long UserId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        public bool IsActive { get; set; } // for logical deletes

        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }
    }
}
