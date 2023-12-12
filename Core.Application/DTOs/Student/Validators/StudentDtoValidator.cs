using Core.Application.Contracts.Persistence;
using Core.Application.Services;
using Core.Application.Transform;
using FluentValidation;
using MajorEntity = Core.Domain.Entities.Major;

namespace Core.Application.DTOs.Student.Validators
{
    public class StudentDtoValidator : AbstractValidator<IStudentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.MajorId)
                .MustAsync(async (id, token) =>
                {
                    var majorEntityExists = await _unitOfWork.Repository<MajorEntity>().GetByIdAsync(id);
                    return majorEntityExists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("majorId", "major"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190));

            RuleFor(x => x.Gender)
                .Must(gender => string.IsNullOrEmpty(gender) || CommonTranform.GetGender().Contains(gender))
                .WithMessage(ValidatorTransform.Must("gender", CommonTranform.GetGender()));

            RuleFor(x => x.DateOfBirth)
                .Must(dateOfBirth => string.IsNullOrEmpty(dateOfBirth.ToString()) || 
                                    CustomValidator.IsAtLeastNYearsOld(dateOfBirth, 16))
                .WithMessage(ValidatorTransform.MustDate("dateOfBirth", 16));

            RuleFor(x => x.PhoneNumber)
                .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length == 10)
                .WithMessage(ValidatorTransform.Length("phoneNumber", 10));

            RuleFor(x => x.Email)
                .Must(email => string.IsNullOrEmpty(email) || CustomValidator.BeValidEmail(email))
                .WithMessage(ValidatorTransform.ValidValue("email"));

            RuleFor(x => x.Class)
                .NotEmpty().WithMessage(ValidatorTransform.Required("Class"));
        }
    }
}
