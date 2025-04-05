namespace ProductService.Products.Validation
{
    using Core;
    using FluentValidation;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrderService.Products;

    public class ProductValidator : BaseValidator<Product, ProductValidator>, IBaseValidator<Product>
    {
        public ProductValidator(ILogger<ProductValidator> logger) : base(logger)
        {
        }

        protected override void SetupValidation()
        {
            RuleFor(product => product.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

            RuleFor(product => product.Price)
                .InclusiveBetween(0.01m, 10000.00m)
                .WithMessage("Price must be between 0.01 and 10,000.00.");
        }
    }
}
