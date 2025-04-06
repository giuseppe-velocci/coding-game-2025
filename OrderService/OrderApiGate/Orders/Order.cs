namespace OrderApiGate.Orders
{
    public class Order
    {
        public long OrderId { get; set; }

        public long UserId { get; set; }

        public long AddressId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
