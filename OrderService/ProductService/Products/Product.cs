using ProductService.Categories;
using System.Text.Json.Serialization;

namespace ProductService.Products
{
    public class Product
    {
        public long ProductId { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public long CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
