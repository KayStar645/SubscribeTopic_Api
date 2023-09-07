using Core.Application.Custom;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Department.Validators
{
    public class DepartmentDtoValidator : AbstractValidator<IDepartmentDto>
    {
        public DepartmentDtoValidator()
        {
            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"));

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage(ValidatorTranform.Required("address"));

            RuleFor(x => x.PhoneNumber)
                .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length == 10)
                .WithMessage(ValidatorTranform.Length("phoneNumber", 10));

            RuleFor(x => x.Email)
                .Must(email => string.IsNullOrEmpty(email) || CustomValidator.BeValidEmail(email))
                .WithMessage(ValidatorTranform.ValidValue("email"));
        }
    }
}
