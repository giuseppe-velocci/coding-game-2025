using Core;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Products;
using ProductService.Products.Validation;

namespace ProductServiceTests
{
    public class ProductValidatorTests
    {
        private readonly IBaseValidator<Product> _sut;

        public ProductValidatorTests()
        {
            _sut = new ProductValidator(new Mock<ILogger<ProductValidator>>().Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Should_Have_Error_When_Name_Is_Null_Or_Empty(string? name)
        {
            // Arrange
            var product = new Product
            {
                Name = name!,
                Price = 100,
                CategoryId = 1
            };

            // Act
            var result = await _sut.ValidateItemAsync(product, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Product name is required.", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Too_Long()
        {
            // Arrange
            var category = new Product
            {
                Name = new string('a', 101),
                Price = 20,
                CategoryId = 1
            };

            // Act
            var result = await _sut.ValidateItemAsync(category, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Product name must not exceed 100 characters.", result.Message);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10_000.1)]
        public async Task Should_Have_Error_When_Price_Is_Out_Of_Range(decimal price)
        {
            // Arrange
            var product = new Product
            {
                Name = "Laptop",
                Price = price,
                CategoryId = 1
            };

            // Act
            var result = await _sut.ValidateItemAsync(product, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Price must be between 0.01 and 10,000.00.", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_CategoryId_Is_Invalid()
        {
            // Arrange
            var product = new Product
            {
                Name = "Laptop",
                Price = 500,
                CategoryId = 0
            };

            // Act
            var result = await _sut.ValidateItemAsync(product, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Category ID must be greater than 0.", result.Message);
        }

        [Fact]
        public async Task Should_Not_Have_Validation_Errors_For_Valid_Product()
        {
            // Arrange
            var product = new Product
            {
                Name = "Laptop",
                Price = 1000,
                CategoryId = 2
            };


            // Act
            var result = await _sut.ValidateItemAsync(product, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.Message);
        }
    }
}
