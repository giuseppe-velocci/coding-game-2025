using Microsoft.EntityFrameworkCore;
using OrderService;
using OrderService.Orders;
using OrderService.Orders.Storage;

namespace OrderServiceTests
{
    public class OrderRepositoryTests
    {
        [Fact]
        public async Task Create_When_User_And_Address_Are_Not_Given_Should_Add_Order()
        {
            //Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new OrderRepository<Exception>(context);
            var order = new Order { UserId = 1, AddressId = 1, OrderDate = DateTime.UtcNow };

            //Act
            var result = await sut.Create(order, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(order.OrderId, result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_Order()
        {
            //Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new OrderRepository<Exception>(context);

            OrderDetail[] orderDetails = InitializeOrderDependencies(context);
            await context.SaveChangesAsync();

            var order = new Order { UserId = 1, AddressId = 1, OrderDate = DateTime.UtcNow };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            //Act
            var result = await sut.ReadOne(order.OrderId, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(order.OrderId, result.Value.OrderId);
        }

        [Fact]
        public async Task ReadAll_Should_Return_All_Orders()
        {
            //Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new OrderRepository<Exception>(context);

            OrderDetail[] orderDetail = InitializeOrderDependencies(context);
            var order1 = new Order { OrderId = 1, UserId = 1, AddressId = 1, OrderDetails = [..orderDetail.Where(x => x.OrderId == 1)], OrderDate = DateTime.UtcNow };
            var order2 = new Order { OrderId = 2, UserId = 2, AddressId = 2, OrderDetails = [..orderDetail.Where(x => x.OrderId == 2)], OrderDate = DateTime.UtcNow };
            context.Orders.AddRange(order1, order2);
            await context.SaveChangesAsync();

            //Act
            var result = await sut.ReadAll(CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Value.Length);
        }

        [Fact]
        public async Task Update_Should_Modify_Order()
        {
            //Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new OrderRepository<Exception>(context);

            OrderDetail[] orderDetail = InitializeOrderDependencies(context);
            var order = new Order
            {
                UserId = 1,
                AddressId = 1,
                OrderDetails = [.. orderDetail.Where(x => x.OrderId == 1)],
                OrderDate = DateTime.UtcNow,
                IsActive = true
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            order.AddressId = 2;

            //Act
            var result = await sut.Update(order.OrderId, order, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            var updatedOrder = await context.Orders.FindAsync(order.OrderId);
            Assert.Equal(2, updatedOrder.AddressId);
        }
        
        [Fact]
        public async Task Update_When_Changing_UserId_Should_Not_Modify_Order()
        {
            //Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new OrderRepository<Exception>(context);

            OrderDetail[] orderDetail = InitializeOrderDependencies(context);
            var order = new Order
            {
                UserId = 1,
                AddressId = 1,
                OrderDetails = [.. orderDetail.Where(x => x.OrderId == 1)],
                OrderDate = DateTime.UtcNow,
                IsActive = true
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            order.UserId = 2;

            //Act
            var result = await sut.Update(order.OrderId, order, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Delete_Should_Deactivate_Order()
        {
            //Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new OrderRepository<Exception>(context);

            var order = new Order { UserId = 1, AddressId = 1, OrderDate = DateTime.UtcNow, IsActive = true };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            //Act
            var result = await sut.Delete(order.OrderId, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            var deletedOrder = await context.Orders.FindAsync(order.OrderId);
            Assert.False(deletedOrder.IsActive);
        }

        private static OrderDetail[] InitializeOrderDependencies(OrderDbContext context)
        {
            OrderDetail[] orderDetails = new OrderDetail[]{
                new() { OrderDetailId = 1, OrderId = 1, ProductId = 1, ProductName = "p1", Quantity = 3 },
                new() { OrderDetailId = 2, OrderId = 1, ProductId = 2, ProductName = "p2", Quantity = 2 },
                new() { OrderDetailId = 3, OrderId = 2, ProductId = 1, ProductName = "p3", Quantity = 1 }
            };
            context.AddRange(orderDetails);
            return orderDetails;
        }

        private static OrderDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new OrderDbContext(options);
        }
    }
}
