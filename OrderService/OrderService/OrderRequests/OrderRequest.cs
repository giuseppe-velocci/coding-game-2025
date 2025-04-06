using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.OrderRequests
{
    public class OrderRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OrderId { get; set; }

        public long UserId { get; set; }

        public long AddressId { get; set; }

        public List<OrderProduct> ProductIds { get; set; } = new();
    }
}
