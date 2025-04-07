using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Users
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }
        
        public bool IsActive { get; set; }
    }
}
