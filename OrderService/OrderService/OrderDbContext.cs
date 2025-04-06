using Microsoft.EntityFrameworkCore;
using OrderService.Addresses;
using OrderService.Orders;
using OrderService.Products;
using OrderService.Users;
using System.Xml;

namespace OrderService
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
