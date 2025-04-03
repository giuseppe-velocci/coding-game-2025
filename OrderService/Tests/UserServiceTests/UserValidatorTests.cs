using UserService.Users.Validation;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using UserService.Users;

namespace UserServiceTests
{
    public class UserValidatorTests
    {
        private readonly UserValidator _validator;

        public UserValidatorTests()
        {
            Mock<ILogger<UserValidator>> _loggerMock = new ();
            _validator = new UserValidator(_loggerMock.Object);
        }

        [Fact]
        public void Validate_Valid_User_Returns_Success()
        {
            // Arrange
            var user = new User { Name = "Valid User", Email = "valid@example.com" };

            // Act
            var result = _validator.Validate(user);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_User_Without_Name_Returns_Failure()
        {
            // Arrange
            var user = new User { Email = "valid@example.com" };

            // Act
            var result = _validator.Validate(user);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "User name is required.");
        }

        [Fact]
        public void Validate_User_With_Long_Name_Returns_Failure()
        {
            // Arrange
            var user = new User { Name = new string('a', 101), Email = "valid@example.com" };

            // Act
            var result = _validator.Validate(user);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "User name must not exceed 100 characters.");
        }

        [Fact]
        public void Validate_User_Without_Email_Returns_Failure()
        {
            // Arrange
            var user = new User { Name = "Valid User" };

            // Act
            var result = _validator.Validate(user);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "User email is required.");
        }

        [Fact]
        public void Validate_User_With_Invalid_Email_Returns_Failure()
        {
            // Arrange
            var user = new User { Name = "Valid User", Email = "invalid-email" };

            // Act
            var result = _validator.Validate(user);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Email must be a valid email address.");
        }
    }
}