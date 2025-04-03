using ProductService.Categories;
using System.Text.Json.Serialization;

namespace ProductService.Products
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
