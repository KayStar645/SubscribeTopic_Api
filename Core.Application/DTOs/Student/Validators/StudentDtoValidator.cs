using Core.Application.Contracts.Persistence;
using Core.Application.Custom;
using Core.Application.Transform;
using FluentValidation;
using MajorEntity = Core.Domain.Entities.Major;
using StudentEntity = Core.Domain.Entities.Student;

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
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("majorId", "major"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var student = await _unitOfWork.Repository<StudentEntity>()
                                        .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return student != null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190));

            RuleFor(x => x.Gender)
                .Must(gender => string.IsNullOrEmpty(gender) || gender == CommonTranform.male 
                            || gender == CommonTranform.female || gender == CommonTranform.other)
                .WithMessage(ValidatorTranform.Must("gender", CommonTranform.GetGender()));

            RuleFor(x => x.DateOfBirth)
                .Must(dateOfBirth => string.IsNullOrEmpty(dateOfBirth.ToString()) || 
                                    CustomValidator.IsAtLeastNYearsOld(dateOfBirth, 16))
                .WithMessage(ValidatorTranform.MustDate("dateOfBirth", 16));

            RuleFor(x => x.PhoneNumber)
                .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length == 10)
                .WithMessage(ValidatorTranform.Length("phoneNumber", 10));

            RuleFor(x => x.Email)
                .Must(email => string.IsNullOrEmpty(email) || CustomValidator.BeValidEmail(email))
                .WithMessage(ValidatorTranform.ValidValue("email"));

            RuleFor(x => x.Class)
                .NotEmpty().WithMessage(ValidatorTranform.Required("Class"));
        }
    }
}
