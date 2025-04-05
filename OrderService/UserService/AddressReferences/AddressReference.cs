using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using UserService.Users;

namespace UserService.AddressReferences
{
    public class AddressReference
    {
        [Key]
        public long AddressId { get; set; }
        public long UserId { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}