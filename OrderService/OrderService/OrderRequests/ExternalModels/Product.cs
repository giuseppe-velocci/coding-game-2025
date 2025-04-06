using ProductService.Categories;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderService.OrderRequests.ExternalModels
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public long CategoryId { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
