using AddressService.Addresses;
using Microsoft.EntityFrameworkCore;

namespace AddressService
{
    public class AddressDbContext(DbContextOptions<AddressDbContext> options) : DbContext(options)
    {
        public DbSet<Address> Addresses { get; set; }

        public void ApplyMigrations(AddressDbContext context)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
    }
}
