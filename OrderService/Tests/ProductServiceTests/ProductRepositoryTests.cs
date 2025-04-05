using Core;
using Microsoft.EntityFrameworkCore;
using ProductService;
using ProductService.Categories;
using ProductService.Products;
using ProductService.Products.Storage;

namespace ProductServiceTests
{
    public class ProductRepositoryTests : IClassFixture<ProductRepositoryFixture>
    {
        private readonly ProductRepositoryFixture _fixture;

        public ProductRepositoryTests(ProductRepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Create_Should_Add_Product_To_Database()
        {
            // Arrange
            Category cat = new() { Name = "Cat1", CategoryId = 10 };
            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Category = cat
            };

            // Act
            var result = await _fixture._sut.Create(product, CancellationToken.None);
            var productFromDb = await _fixture._context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);

            // Assert
            Assert.IsType<SuccessResult<long>>(result);
            Assert.NotNull(productFromDb);
            Assert.Equal("Test Product", productFromDb.Name);
            Assert.Equal(1, productFromDb.ProductId);
        }

        [Fact]
        public async Task ReadAll_Should_Return_All_Products_From_Database()
        {
            //Arrange
            var fixture = new ProductRepositoryFixture();

            Category cat = new() { Name = "Cat2", CategoryId = 20 };
            Product[] entities =
            {
                new Product { ProductId = 10, CategoryId = 20, Category = cat, Name = "P1" },
                new Product { ProductId = 20, CategoryId = 20, Category = cat, Name = "P2" },
                new Product { ProductId = 30, CategoryId = 20, Category = cat, Name = "P3" }
            };
            fixture._context.Products.AddRange(entities);
            fixture._context.SaveChanges();

            //Act
            var result = await fixture._sut.ReadAll(CancellationToken.None);

            //Assert
            Assert.Equal(entities, result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_Product_When_Id_Exists()
        {
            // Arrange
            Category cat = new() { Name = "Cat1", CategoryId = 40 };
            var product = new Product { ProductId = 6, CategoryId = 1, Category = cat, Name = "P1" };
            _fixture._context.Add(product);
            _fixture._context.SaveChanges();

            // Act
            var result = await _fixture._sut.ReadOne(6, CancellationToken.None);

            // Assert
            Assert.IsType<SuccessResult<Product>>(result);
            Assert.Equal(product, result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_NotFound_When_Id_Does_Not_Exist()
        {
            // Act
            var result = await _fixture._sut.ReadOne(999, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult<Product>>(result);
            Assert.Equal("Product 999 not found", result.Message);
        }

        [Fact]
        public async Task Update_Should_Modify_Product_When_Id_Exists()
        {
            // Arrange
            string expectedName = "P22";
            int expectedCategoryId = 90;
            Category cat = new() { Name = "Cat1", CategoryId = 60 };
            Category cat1 = new() { Name = "Cat2", CategoryId = expectedCategoryId };
            var product = new Product { ProductId = 9, CategoryId = 1, Category = cat, Name = "P1" };
            _fixture._context.Add(product);
            _fixture._context.SaveChanges();
            Product newProduct = new() { CategoryId = cat1.CategoryId, Category = cat, Name = expectedName };

            // Act
            var result = await _fixture._sut.Update(9, newProduct, CancellationToken.None);
            var updated = await _fixture._context.Products.FirstAsync(x => x.ProductId == product.ProductId);

            // Assert
            Assert.IsType<SuccessResult<None>>(result);
            Assert.Equal(expectedName, updated.Name);
            Assert.Equal(expectedCategoryId, updated.CategoryId);
        }

        [Fact]
        public async Task Delete_Should_Remove_Product_When_Id_Exists()
        {
            // Arrange
            Category cat = new() { Name = "Cat1", CategoryId = 150 };
            var product = new Product { ProductId = 600, CategoryId = 150, Category = cat, Name = "P1" };
            _fixture._context.Add(product);
            _fixture._context.SaveChanges();

            // Act
            var result = await _fixture._sut.Delete(product.ProductId, CancellationToken.None);
            var found = await _fixture._context.Products.FirstOrDefaultAsync(x => x.ProductId == product.ProductId);

            // Assert
            Assert.IsType<SuccessResult<None>>(result);
            Assert.Null(found);
        }
    }

    public class ProductRepositoryFixture : IDisposable
    {
        public ProductDbContext _context { get; private set; }
        public ProductRepository _sut { get; private set; }

        public ProductRepositoryFixture()
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