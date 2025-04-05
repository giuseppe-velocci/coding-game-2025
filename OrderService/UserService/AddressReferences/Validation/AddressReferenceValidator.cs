namespace AddressReferenceService.AddressReferences.Validation
{
    using Core;
    using FluentValidation;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using UserService.AddressReferences;

    public class AddressReferenceValidator : BaseValidator<AddressReference, AddressReferenceValidator>, IBaseValidator<AddressReference>
    {
        public AddressReferenceValidator(ILogger<AddressReferenceValidator> logger) : base(logger)
        {
        }

        protected override void SetupValidation()
        {
            RuleFor(address => address.AddressId)
                .GreaterThan(0)
                .WithMessage("AddressId must be greater than 0.");

            RuleFor(address => address.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be greater than 0.");
        }
    }
}
