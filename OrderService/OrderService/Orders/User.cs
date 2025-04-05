using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderService.Orders
{
    public class User
    {
        [Key]
        public long UserId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }
    }
}
