using AddressService.Addresses;
using FluentValidation;
using Infrastructure;

namespace AddressService.Addresses.Validation
{
    public class AddressValidator : BaseValidator<Address, AddressValidator>
    {
        public AddressValidator(ILogger<AddressValidator> logger) : base(logger)
        {
        }

        protected override void SetupValidation()
        {
            RuleFor(address => address.Street)
                .NotEmpty().WithMessage("Street is required.")
                .MaximumLength(200).WithMessage("Street cannot exceed 200 characters.");

            RuleFor(address => address.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

            RuleFor(address => address.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

            RuleFor(address => address.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(50).WithMessage("Country cannot exceed 50 characters.");

            RuleFor(address => address.ZipCode)
                .MaximumLength(20).WithMessage("ZipCode cannot exceed 20 characters.");
        }
    }
}
