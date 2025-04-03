using ProductService.Products;

namespace ProductService.Categories
{
    public class Category
    {
        public long CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
