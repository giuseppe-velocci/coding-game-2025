using FluentValidation;
using Infrastructure;

namespace OrderService.OrderRequests.Validation
{
    public partial class OrderRequestValidator
    {
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
