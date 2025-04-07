using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserService;
using UserService.Users;
using UserService.Users.Storage;
using Xunit;

namespace UserServiceTests
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task Create_ShouldAddUser()
        {
            //Arrange
            var context = CreateInMemoryContext();
            var repository = new UserRepository<Exception>(context);
            var user = new User { Name = "Test User", Email = "test@example.com", IsActive = true };

            //Act
            var result = await repository.Create(user, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(user.UserId, result.Value);
        }

        [Fact]
        public async Task ReadOne_ShouldReturnUser()
        {
            //Arrange
            var context = CreateInMemoryContext();
            var user = new User { Name = "Test User", Email = "test@example.com", IsActive = true };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository<Exception>(context);
            
            //Act
            var result = await repository.ReadOne(user.UserId, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(user.UserId, result.Value.UserId);
        }

        [Fact]
        public async Task ReadAll_ShouldReturnAllUsers()
        {
            //Arrange
            var context = CreateInMemoryContext();
            var user1 = new User { Name = "Test User 1", Email = "test1@example.com", IsActive = true };
            var user2 = new User { Name = "Test User 2", Email = "test2@example.com", IsActive = true };
            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var repository = new UserRepository<Exception>(context);
            
            //Act
            var result = await repository.ReadAll(CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Value.Length);
        }

        [Fact]
        public async Task Update_Should_Modify_User()
        {
            //Arrange
            var context = CreateInMemoryContext();
            var user = new User { Name = "Test User", Email = "test@example.com", IsActive = true };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository<Exception>(context);
            user.Name = "Updated User";
            
            //Act
            var result = await repository.Update(user.UserId, user, CancellationToken.None);
            var updatedUser = await context.Users.FindAsync(user.UserId);

            //Assert
            Assert.True(result.Success);
            Assert.Equal("Updated User", updatedUser.Name);
        }

        [Fact]
        public async Task Delete_ShouldDeactivateUser()
        {
            //Arrange
            var context = CreateInMemoryContext();
            var user = new User { Name = "Test User", Email = "test@example.com", IsActive = true };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository<Exception>(context);
            
            //Act
            var result = await repository.Delete(user.UserId, CancellationToken.None);

            //Assert
            Assert.True(result.Success);
            var deletedUser = await context.Users.FindAsync(user.UserId);
            Assert.False(deletedUser.IsActive);
        }

        private static UserDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "UserDatabase")
                .Options;
            return new UserDbContext(options);
        }
    }
}