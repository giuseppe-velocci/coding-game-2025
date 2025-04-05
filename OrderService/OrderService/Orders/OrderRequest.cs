namespace OrderService.Orders
{
    public class OrderRequest
    {
        public long OrderId { get; set; }

        public long UserId { get; set; }

        public List<OrderProduct> ProductIds { get; set; } = new ();

        public long AddressId { get; set; }
    }

    public class OrderProduct
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
