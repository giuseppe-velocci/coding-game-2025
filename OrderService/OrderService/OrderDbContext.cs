using Microsoft.EntityFrameworkCore;
using OrderService.Addresses;
using OrderService.Orders;
using OrderService.Products;
using OrderService.Users;

namespace OrderService
{
    public class OrderDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
