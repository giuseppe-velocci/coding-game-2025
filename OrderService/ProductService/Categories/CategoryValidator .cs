using FluentValidation;
using Infrastructure;

namespace ProductService.Categories
{
    public class CategoryValidator : BaseValidator<Category, CategoryValidator>
    {
        public CategoryValidator(ILogger<CategoryValidator> logger) : base(logger)
        {
        }

        protected override void SetupValidation()
        {
            RuleFor(category => category.CategoryId)
            .GreaterThan(-1).WithMessage("Category ID must be greater or equal than 0.");

            RuleFor(category => category.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");
        }
    }
}
