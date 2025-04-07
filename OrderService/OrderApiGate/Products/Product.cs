namespace OrderApiGate.Products
{
    public class Product : WriteProduct
    {
        public long ProductId { get; set; }
        public ProductCategory Category { get; set; }
    }
}
