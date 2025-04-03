namespace OrderApiGate.Products
{
    public class WriteProduct
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public long CategoryId { get; set; }
    }
}
