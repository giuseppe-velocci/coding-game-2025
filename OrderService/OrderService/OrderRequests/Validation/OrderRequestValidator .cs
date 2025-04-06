using FluentValidation;
using Infrastructure;
using static OrderService.OrderRequests.Validation.OrderRequestValidator;

namespace OrderService.OrderRequests.Validation
{
    public partial class OrderRequestValidator(
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
    }
}
