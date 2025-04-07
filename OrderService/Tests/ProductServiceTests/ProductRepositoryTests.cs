using Core;
using Microsoft.EntityFrameworkCore;
using ProductService;
using ProductService.Categories;
using ProductService.Products;
using ProductService.Products.Storage;

namespace ProductServiceTests
{
    public class ProductRepositoryTests
    {
        [Fact]
        public async Task Create_Should_Add_Product_To_Database()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            ProductRepository<Exception> sut = new(context);

            Category cat = new() { Name = "Cat1", CategoryId = 10, IsActive = true };
            var product = new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Price = 1,
                CategoryId = cat.CategoryId
            };
            context.Categories.Add(cat);
            context.SaveChanges();

            // Act
            var result = await sut.Create(product, CancellationToken.None);
            var productFromDb = await context.Products.FirstOrDefaultAsync(p => p.ProductId == 1);

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
            var context = CreateInMemoryDbContext();
            ProductRepository<Exception> sut = new(context);

            Category cat = new() { Name = "Cat2", CategoryId = 20 };
            Product[] entities =
            {
                new Product { ProductId = 10, CategoryId = 20, Category = cat, Name = "P1" },
                new Product { ProductId = 20, CategoryId = 20, Category = cat, Name = "P2" },
                new Product { ProductId = 30, CategoryId = 20, Category = cat, Name = "P3" }
            };
            context.Products.AddRange(entities);
            context.SaveChanges();

            //Act
            var result = await sut.ReadAll(CancellationToken.None);

            //Assert
            Assert.Equal(entities, result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_Product_When_Id_Exists()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            ProductRepository<Exception> sut = new(context);

            Category cat = new() { Name = "Cat1", CategoryId = 40 };
            var product = new Product { ProductId = 6, CategoryId = 1, Category = cat, Name = "P1" };
            context.Add(product);
            context.SaveChanges();

            // Act
            var result = await sut.ReadOne(6, CancellationToken.None);

            // Assert
            Assert.IsType<SuccessResult<Product>>(result);
            Assert.Equal(product, result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_NotFound_When_Id_Does_Not_Exist()
        {
            // Act
            var context = CreateInMemoryDbContext();
            ProductRepository<Exception> sut = new(context);

            var result = await sut.ReadOne(999, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult<Product>>(result);
            Assert.Equal("Product 999 not found", result.Message);
        }

        [Fact]
        public async Task Update_Should_Modify_Product_When_Id_Exists()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            ProductRepository<Exception> sut = new(context);

            string expectedName = "P22";
            int expectedCategoryId = 90;
            Category cat = new() { Name = "Cat1", CategoryId = 60, IsActive = true };
            Category cat1 = new() { Name = "Cat2", CategoryId = expectedCategoryId, IsActive = true };
            Product product = new() { ProductId = 9, CategoryId = 1, Category = cat, Name = "P1" };
            context.Categories.AddRange(cat, cat1);
            context.Products.Add(product);
            context.SaveChanges();
            Product newProduct = new()
            {
                CategoryId = cat1.CategoryId,
                Category = cat,
                Name = expectedName
            };

            // Act
            var result = await sut.Update(9, newProduct, CancellationToken.None);
            var updated = await context.Products.FirstAsync(x => x.ProductId == product.ProductId);

            // Assert
            Assert.IsType<SuccessResult<None>>(result);
            Assert.Equal(expectedName, updated.Name);
            Assert.Equal(expectedCategoryId, updated.CategoryId);
        }

        [Fact]
        public async Task Delete_Should_Remove_Product_When_Id_Exists()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            ProductRepository<Exception> sut = new(context);

            Category cat = new() { Name = "Cat1", CategoryId = 150 };
            var product = new Product { ProductId = 600, CategoryId = 150, Category = cat, Name = "P1" };
            context.Add(product);
            context.SaveChanges();

            // Act
            var result = await sut.Delete(product.ProductId, CancellationToken.None);
            var found = await context.Products.FirstOrDefaultAsync(x => x.ProductId == product.ProductId);

            // Assert
            Assert.IsType<SuccessResult<None>>(result);
            Assert.Null(found);
        }
    
        private static ProductDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ProductDbContext(options);
        }
    }
}