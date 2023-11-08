using FluentValidation;
using Core.Application.Transform;
using Core.Application.Contracts.Persistence;
using DepartmentEntity = Core.Domain.Entities.Department;
using TeacherEntity = Core.Domain.Entities.Teacher;
using Core.Application.Services;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class TeacherDtoValidator : AbstractValidator<ITeacherDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public TeacherDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.DepartmentId)
                .MustAsync(async (id, token) =>
                {
                    var teacherExists = await _unitOfWork.Repository<DepartmentEntity>().GetByIdAsync(id);
                    return teacherExists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("departmentId", "departments"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190));

            RuleFor(x => x.Gender)
                .Must(gender => string.IsNullOrEmpty(gender) || CommonTranform.GetGender().Any(x => x.Equals(gender)))
                .WithMessage(ValidatorTransform.Must("gender", CommonTranform.GetGender()));

            RuleFor(x => x.DateOfBirth)
                .Must(dateOfBirth => string.IsNullOrEmpty(dateOfBirth.ToString()) || CustomValidator.IsAtLeastNYearsOld(dateOfBirth, 16))
                .WithMessage(ValidatorTransform.MustDate("dateOfBirth", 16));

            RuleFor(x => x.PhoneNumber)
                .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length == 10)
                .WithMessage(ValidatorTransform.Length("phoneNumber", 10));

            RuleFor(x => x.Email)
                .Must(email => string.IsNullOrEmpty(email) || CustomValidator.BeValidEmail(email))
                .WithMessage(ValidatorTransform.ValidValue("email"));

            RuleFor(x => x.AcademicTitle)
                .Must(academicTitle => string.IsNullOrEmpty(academicTitle) || CommonTranform.GetListAcademicTitle().Any(x => x.Equals(academicTitle)))
                .WithMessage(ValidatorTransform.Must("academicTitle", CommonTranform.GetListAcademicTitle()));

            RuleFor(x => x.Degree)
                .Must((dto, degree) =>
                    string.IsNullOrEmpty(dto.AcademicTitle) || dto.AcademicTitle != CommonTranform.doctorate
                    ? string.IsNullOrEmpty(degree)
                    : CommonTranform.GetListDegree().Any(x => x.Equals(degree)) || string.IsNullOrWhiteSpace(degree))
                .WithMessage(ValidatorTransform.MustWhen("degree", CommonTranform.GetListDegree(), "academicTitle", CommonTranform.doctorate));
        }    
    }
}
