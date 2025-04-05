using Core;
using Microsoft.EntityFrameworkCore;
using ProductService;
using ProductService.Categories;
using ProductService.Categories.Storage;

namespace ProductServiceTests
{
    public class CategoryRepositoryTests : IClassFixture<CategoryRepositoryFixture>
    {
        private readonly CategoryRepositoryFixture _fixture;

        public CategoryRepositoryTests(CategoryRepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Create_Should_Add_Category_To_Database()
        {
            // Arrange
            Category cat = new() { Name = "Cat1", CategoryId = 10 };
            var product = new Category
            {
                CategoryId = 1,
                Name = "Test Category",
            };

            // Act
            var result = await _fixture._sut.Create(product, CancellationToken.None);
            var productFromDb = await _fixture._context.Categories.FirstOrDefaultAsync(p => p.CategoryId == 1);

            // Assert
            Assert.IsType<SuccessResult<long>>(result);
            Assert.NotNull(productFromDb);
            Assert.Equal("Test Category", productFromDb.Name);
            Assert.Equal(1, productFromDb.CategoryId);
        }

        [Fact]
        public async Task ReadAll_Should_Return_All_Categories_From_Database()
        {
            //Arrange
            var fixture = new CategoryRepositoryFixture();

            Category[] entities =
            {
                new Category { CategoryId = 10, Name = "P1" },
                new Category { CategoryId = 20, Name = "P2" },
                new Category { CategoryId = 30, Name = "P3" }
            };
            fixture._context.Categories.AddRange(entities);
            fixture._context.SaveChanges();

            //Act
            var result = await fixture._sut.ReadAll(CancellationToken.None);

            //Assert
            Assert.Equal(entities, result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_Category_When_Id_Exists()
        {
            // Arrange
            var categoryId = 40;
            Category cat = new() { Name = "Cat1", CategoryId = categoryId };
            _fixture._context.Add(cat);
            _fixture._context.SaveChanges();

            // Act
            var result = await _fixture._sut.ReadOne(categoryId, CancellationToken.None);

            // Assert
            Assert.IsType<SuccessResult<Category>>(result);
            Assert.Equal(cat, result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_NotFound_When_Id_Does_Not_Exist()
        {
            // Act
            var result = await _fixture._sut.ReadOne(999, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult<Category>>(result);
            Assert.Equal("Category 999 not found", result.Message);
        }

        [Fact]
        public async Task Update_Should_Modify_Category_When_Id_Exists()
        {
            // Arrange
            string expectedName = "Cat2";
            int categoryId = 90;
            Category cat = new() { Name = "Cat1", CategoryId = categoryId };
            Category cat1 = new() { Name = expectedName };
            _fixture._context.Add(cat);
            _fixture._context.SaveChanges();

            // Act
            var result = await _fixture._sut.Update(categoryId, cat1, CancellationToken.None);
            var updated = await _fixture._context.Categories.FirstAsync(x => x.CategoryId == cat.CategoryId);

            // Assert
            Assert.IsType<SuccessResult<None>>(result);
            Assert.Equal(expectedName, updated.Name);
        }

        [Fact]
        public async Task Delete_Should_Remove_Category_When_Id_Exists()
        {
            // Arrange
            Category cat = new() { Name = "Cat1", CategoryId = 150 };
            _fixture._context.Add(cat);
            _fixture._context.SaveChanges();

            // Act
            var result = await _fixture._sut.Delete(cat.CategoryId, CancellationToken.None);
            var found = await _fixture._context.Categories.FirstOrDefaultAsync(x => x.CategoryId == cat.CategoryId);

            // Assert
            Assert.IsType<SuccessResult<None>>(result);
            Assert.Null(found);
        }
    }

    public class CategoryRepositoryFixture : IDisposable
    {
        public ProductDbContext _context { get; private set; }
        public CategoryRepository _sut { get; private set; }

        public CategoryRepositoryFixture()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Initialize context
            _context = new(options);

            // Initialize the SUT
            _sut = new(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}