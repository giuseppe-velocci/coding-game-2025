using Core;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Categories;
using ProductService.Categories.Validation;

namespace ProductServiceTests
{
    public class CategoryValidatorTests
    {
        private readonly IBaseValidator<Category> _sut;

        public CategoryValidatorTests()
        {
            _sut = new CategoryValidator(new Mock<ILogger<CategoryValidator>>().Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Should_Have_Error_When_Name_Is_Null_Or_Empty(string? name)
        {
            // Arrange
            var category = new Category
            {
                Name = name!,
                CategoryId = 1
            };

            // Act
            var result = await _sut.ValidateItemAsync(category, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Category name is required.", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_Name_Is_Too_Long()
        {
            // Arrange
            var category = new Category
            {
                Name = new string('a', 101),
                CategoryId = 0
            };

            // Act
            var result = await _sut.ValidateItemAsync(category, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Category name must not exceed 100 characters.", result.Message);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        public async Task Should_Have_Error_When_CategoryId_Is_Out_Of_Range(long id)
        {
            // Arrange
            var category = new Category
            {
                Name = "Cat",
                CategoryId = id
            };

            // Act
            var result = await _sut.ValidateItemAsync(category, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Category ID must be greater or equal than 0.", result.Message);
        }

        [Fact]
        public async Task Should_Not_Have_Validation_Errors_For_Valid_Category()
        {
            // Arrange
            var category = new Category
            {
                Name = "Laptop",
                CategoryId = 2
            };


            // Act
            var result = await _sut.ValidateItemAsync(category, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.Message);
        }
    }
}
