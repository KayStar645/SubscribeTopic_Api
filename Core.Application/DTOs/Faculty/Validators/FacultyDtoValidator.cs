using Core.Application.Contracts.Persistence;
using Core.Application.Custom;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Faculty.Validators
{
    public class FacultyDtoValidator : AbstractValidator<IFacultyDto>
    {
        private readonly IUnitOfWork unitOfWork;

        public FacultyDtoValidator(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;

            RuleFor(x => x.PhoneNumber)
                .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length == 10)
                .WithMessage(ValidatorTranform.Length("phoneNumber", 10));

            RuleFor(x => x.Email)
                .Must(email => string.IsNullOrEmpty(email) || CustomValidator.BeValidEmail(email))
                .WithMessage(ValidatorTranform.ValidValue("email"));
        }
    }
}
