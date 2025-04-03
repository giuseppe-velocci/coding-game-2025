namespace OrderApiGate.Orders
{
    public class WriteOrder
    {
        public long UserId { get; set; }

        public long AddressId { get; set; }

        public List<WriteOrderProduct> ProductIds { get; set; } = new();
    }
}
