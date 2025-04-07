using AddressService.Addresses;
using AddressService.Addresses.Validation;
using Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AddressServiceTests
{
    public class AddressValidatorTests
    {
        private readonly IBaseValidator<Address>_validator;

        public AddressValidatorTests()
        {
            Mock<ILogger<AddressValidator>> loggerMock = new();
            _validator = new AddressValidator(loggerMock.Object);
        }

        [Fact]
        public async Task Should_Have_Error_When_Street_Is_Empty()
        {
            //Arrange
            var address = new Address { Street = string.Empty };

            //Act
            var result = await _validator.ValidateItemAsync(address, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.Contains("Street is required.", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_Street_Exceeds_MaxLength()
        {
            //Arrange
            var address = new Address { Street = new string('a', 201) };

            //Act
            var result = await _validator.ValidateItemAsync(address, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.Contains("Street cannot exceed 200 characters.", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_City_Is_Empty()
        {
            //Arrange
            var address = new Address { City = string.Empty };

            //Act
            var result = await _validator.ValidateItemAsync(address, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.Contains("City is required.", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_City_Exceeds_MaxLength()
        {
            //Arrange
            var address = new Address { City = new string('a', 101) };

            //Act
            var result = await _validator.ValidateItemAsync(address, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.Contains("City cannot exceed 100 characters.", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_State_Is_Empty()
        {
            //Arrange
            var address = new Address { State = string.Empty };

            //Act
            var result = await _validator.ValidateItemAsync(address, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.Contains("State is required", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_State_Exceeds_MaxLength()
        {
            //Arrange
            var address = new Address { State = new string('a', 101) };

            //Act
            var result = await _validator.ValidateItemAsync(address, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.Contains("State cannot exceed 100 characters", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_Country_Is_Empty()
        {
            //Arrange
            var address = new Address { Country = string.Empty };
            
            //Act
            var result = await _validator.ValidateItemAsync(address, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.Contains("Country is required", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_Country_Exceeds_MaxLength()
        {
            //Arrange
            var address = new Address { Country = new string('a', 51) };
            
            //Act
            var result = await _validator.ValidateItemAsync(address, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.Contains("Country cannot exceed 50 characters", result.Message);
        }

        [Fact]
        public async Task Should_Have_Error_When_ZipCode_Exceeds_MaxLength()
        {
            //Arrange
            var address = new Address { ZipCode = new string('a', 21) };

            //Act
            var result = await _validator.ValidateItemAsync(address, CancellationToken.None);

            //Assert
            Assert.False(result.Success);
            Assert.Contains("ZipCode cannot exceed 20 characters", result.Message);
        }

        [Fact]
        public async Task Should_Not_Have_Error_When_Address_Is_Valid()
        {
            //Arrange
            var address = new Address
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "Anystate",
                Country = "AnyCountry",
                ZipCode = "12345"
            };
            
            //Act
            var result = await _validator.ValidateItemAsync(address, CancellationToken.None);
            
            //Assert
            Assert.Empty(result.Message);
        }
    }
}
