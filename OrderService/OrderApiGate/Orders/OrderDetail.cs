namespace OrderApiGate.Orders
{
    public class OrderDetail
    {
        public long OrderDetailId { get; set; }

        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
