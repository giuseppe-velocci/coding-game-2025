using Microsoft.Extensions.Logging;
using Moq;
using OrderService.OrderRequests.Validation;
using FluentValidation.TestHelper;
using static OrderService.OrderRequests.Validation.OrderRequestValidator;
using OrderService.OrderRequests;

namespace OrderServiceTests
{
    public class OrderRequestValidatorTests
    {
        private readonly OrderRequestValidator _validator;
        
        public OrderRequestValidatorTests()
        {
            Mock<ILogger<OrderRequestValidator>> _loggerMock = new ();
            Mock<ILogger<OrderProductValidator>> _productLoggerMock = new ();
            _validator = new OrderRequestValidator(_loggerMock.Object, _productLoggerMock.Object);
        }

        [Fact]
        public void Should_Have_Error_When_UserId_Is_Zero()
        {
            var orderRequest = new OrderRequest { UserId = 0 };
            var result = _validator.TestValidate(orderRequest);
            result.ShouldHaveValidationErrorFor(order => order.UserId);
        }

        [Fact]
        public void Should_Have_Error_When_AddressId_Is_Zero()
        {
            var orderRequest = new OrderRequest { AddressId = 0 };
            var result = _validator.TestValidate(orderRequest);
            result.ShouldHaveValidationErrorFor(order => order.AddressId);
        }

        [Fact]
        public void Should_Have_Error_When_ProductIds_Is_Empty()
        {
            var orderRequest = new OrderRequest { ProductIds = new List<OrderProduct>() };
            var result = _validator.TestValidate(orderRequest);
            result.ShouldHaveValidationErrorFor(order => order.ProductIds);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid_OrderRequest()
        {
            var orderRequest = new OrderRequest
            {
                UserId = 1,
                AddressId = 1,
                ProductIds = new List<OrderProduct> { new OrderProduct { ProductId = 1, Quantity = 1 } }
            };
            var result = _validator.TestValidate(orderRequest);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
