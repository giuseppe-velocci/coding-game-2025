using FluentValidation;
using Infrastructure;
using OrderService.OrderRequests;
using static OrderService.OrderRequests.Validation.OrderRequestValidator;

namespace OrderService.OrderRequests.Validation
{
    public class OrderRequestValidator(
        ILogger<OrderRequestValidator> _logger,
        ILogger<OrderProductValidator> _plogger) :
        BaseValidator<OrderRequest, OrderRequestValidator>(_logger)
    {
        protected override void SetupValidation()
        {
            RuleFor(order => order.OrderId)
            .GreaterThan(0).WithMessage("OrderId must be greater than 0.");

            RuleFor(order => order.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(order => order.AddressId)
                .GreaterThan(0).WithMessage("AddressId must be greater than 0.");

            RuleFor(order => order.ProductIds)
                .NotEmpty().WithMessage("ProductIds must not be empty.")
                .ForEach(product =>
                {
                    product.SetValidator(new OrderProductValidator(_plogger));
                });
        }

        public class OrderProductValidator(ILogger<OrderProductValidator> _logger) :
            BaseValidator<OrderProduct, OrderProductValidator>(_logger)
        {
            protected override void SetupValidation()
            {
                RuleFor(product => product.ProductId)
                    .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

                RuleFor(product => product.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            }
        }
    }
}
