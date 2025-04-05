namespace OrderService.Users.Validation
{
    using Core;
    using FluentValidation;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrderService.Users;

    public class UserValidator : BaseValidator<User, UserValidator>, IBaseValidator<User>
    {
        public UserValidator(ILogger<UserValidator> logger) : base(logger)
        {
        }

        protected override void SetupValidation()
        {
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("User name is required.")
                .MaximumLength(100).WithMessage("User name must not exceed 100 characters.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("User email is required.")
                .MaximumLength(100).WithMessage("User email must not exceed 100 characters.")
                .EmailAddress().WithMessage("Email must be a valid email address.");
        }
    }
}
