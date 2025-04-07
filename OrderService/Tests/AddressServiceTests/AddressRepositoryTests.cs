using AddressService.Addresses.Storage;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using AddressService;
using AddressService.Addresses;

namespace AddressServiceTests
{
    public class AddressRepositoryTests
    {
        [Fact]
        public async Task Create_Should_Return_SuccessResult_When_Address_Is_Created()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new AddressRepository<Exception>(context);

            var address = new Address { Street = "123 Main St", City = "Anytown", State = "CA", Country = "USA", ZipCode = "12345" };
            var cts = new CancellationToken();

            // Act
            var result = await sut.Create(address, cts);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(address.AddressId, result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_SuccessResult_When_Address_Is_Found()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new AddressRepository<Exception>(context);

            var address = new Address { AddressId = 1, Street = "123 Main St", City = "Anytown", State = "CA", Country = "USA", ZipCode = "12345" };
            context.Addresses.Add(address);
            context.SaveChanges();
            var cts = new CancellationToken();

            // Act
            var result = await sut.ReadOne(1, cts);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(address, result.Value);
        }

        [Fact]
        public async Task ReadAll_Should_Return_SuccessResult_When_Addresses_Are_Found()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new AddressRepository<Exception>(context);

            var addresses = new[] { 
                new Address { AddressId = 1, Street = "123 Main St", City = "Anytown", State = "CA", Country = "USA", ZipCode = "12345" } 
            };
            context.Addresses.AddRange(addresses);
            context.SaveChanges();
            var cts = new CancellationToken();

            // Act
            var result = await sut.ReadAll(cts);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(addresses, result.Value);
        }

        [Fact]
        public async Task Update_Should_Return_SuccessResult_When_Address_Is_Updated()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new AddressRepository<Exception>(context);

            var address = new Address { AddressId = 1, Street = "123 Main St", City = "Anytown", State = "CA", Country = "USA", ZipCode = "12345", IsActive = true };
            context.Addresses.Add(address);
            context.SaveChanges();
            var updatedAddress = new Address { Street = "456 Elm St", City = "Othertown", State = "NY", Country = "USA", ZipCode = "67890" };
            var cts = new CancellationToken();

            // Act
            var result = await sut.Update(1, updatedAddress, cts);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task Delete_Should_Return_SuccessResult_When_Address_Is_Deleted()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new AddressRepository<Exception>(context);

            var address = new Address { AddressId = 1, Street = "123 Main St", City = "Anytown", State = "CA", Country = "USA", ZipCode = "12345", IsActive = true };
            context.Addresses.Add(address);
            context.SaveChanges();
            var cts = new CancellationToken();

            // Act
            var result = await sut.Delete(1, cts);

            // Assert
            Assert.True(result.Success);
        }

        private static AddressDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AddressDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AddressDbContext(options);
        }
    }
}
