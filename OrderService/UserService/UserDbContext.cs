using Microsoft.EntityFrameworkCore;
using UserService.AddressReferences;
using UserService.Users;

namespace UserService
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AddressReference> AddressReferences { get; set; }

        public void ApplyMigrations(UserDbContext context)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
    }
}
