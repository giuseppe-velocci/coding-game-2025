using Core;
using Microsoft.EntityFrameworkCore;
using ProductService;
using ProductService.Categories;
using ProductService.Categories.Storage;

namespace ProductServiceTests
{
    public class CategoryRepositoryTests
    {
        [Fact]
        public async Task Create_Should_Add_Category_To_Database()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new CategoryRepository(context);

            Category cat = new() { Name = "Cat1", CategoryId = 10 };
            var product = new Category
            {
                CategoryId = 1,
                Name = "Test Category",
            };

            // Act
            var result = await sut.Create(product, CancellationToken.None);
            var productFromDb = await context.Categories.FirstOrDefaultAsync(p => p.CategoryId == 1);

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
            using var context = CreateInMemoryDbContext();
            var sut = new CategoryRepository(context);

            Category[] entities =
            {
                new Category { CategoryId = 10, Name = "P1" },
                new Category { CategoryId = 20, Name = "P2" },
                new Category { CategoryId = 30, Name = "P3" }
            };
            context.Categories.AddRange(entities);
            context.SaveChanges();

            //Act
            var result = await sut.ReadAll(CancellationToken.None);

            //Assert
            Assert.Equal(entities, result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_Category_When_Id_Exists()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new CategoryRepository(context);

            var categoryId = 40;
            Category cat = new() { Name = "Cat1", CategoryId = categoryId };
            context.Add(cat);
            context.SaveChanges();

            // Act
            var result = await sut.ReadOne(categoryId, CancellationToken.None);

            // Assert
            Assert.IsType<SuccessResult<Category>>(result);
            Assert.Equal(cat, result.Value);
        }

        [Fact]
        public async Task ReadOne_Should_Return_NotFound_When_Id_Does_Not_Exist()
        {
            //Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new CategoryRepository(context);

            // Act
            var result = await sut.ReadOne(999, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult<Category>>(result);
            Assert.Equal("Category 999 not found", result.Message);
        }

        [Fact]
        public async Task Update_Should_Modify_Category_When_Id_Exists()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new CategoryRepository(context);

            string expectedName = "Cat2";
            int categoryId = 90;
            Category cat = new() { Name = "Cat1", CategoryId = categoryId };
            Category cat1 = new() { Name = expectedName };
            context.Add(cat);
            context.SaveChanges();

            // Act
            var result = await sut.Update(categoryId, cat1, CancellationToken.None);
            var updated = await context.Categories.FirstAsync(x => x.CategoryId == cat.CategoryId);

            // Assert
            Assert.IsType<SuccessResult<None>>(result);
            Assert.Equal(expectedName, updated.Name);
        }

        [Fact]
        public async Task Delete_Should_Remove_Category_When_Id_Exists()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var sut = new CategoryRepository(context);

            Category cat = new() { Name = "Cat1", CategoryId = 150 };
            context.Add(cat);
            context.SaveChanges();

            // Act
            var result = await sut.Delete(cat.CategoryId, CancellationToken.None);
            var found = await context.Categories.FirstOrDefaultAsync(x => x.CategoryId == cat.CategoryId);

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