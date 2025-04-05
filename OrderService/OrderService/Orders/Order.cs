using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace OrderService.Orders
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OrderId { get; set; }

        public long UserId { get; set; }

        public long AddressId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("AddressId")]
        public Address Address { get; set; }

        [JsonIgnore]
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
