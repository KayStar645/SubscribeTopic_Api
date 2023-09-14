using FluentValidation;
using Core.Application.Custom;
using Core.Application.Transform;
using Core.Application.Contracts.Persistence;
using DepartmentEntity = Core.Domain.Entities.Department;

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
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("departmentId", "departments"));

            // Chưa validator trùng code và name trong 1 bảng
            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190));

            RuleFor(x => x.Gender)
                .Must(gender => gender == CommonTranform.male || gender == CommonTranform.female || gender == CommonTranform.other)
                .WithMessage(ValidatorTranform.Must("gender", CommonTranform.GetGender()));

            RuleFor(x => x.DateOfBirth)
                .Must(dateOfBirth => CustomValidator.IsAtLeastNYearsOld(dateOfBirth, 16))
                .WithMessage(ValidatorTranform.MustDate("dateOfBirth", 16));

            RuleFor(x => x.PhoneNumber)
                .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length == 10)
                .WithMessage(ValidatorTranform.Length("phoneNumber", 10));

            RuleFor(x => x.Email)
                .Must(email => string.IsNullOrEmpty(email) || CustomValidator.BeValidEmail(email))
                .WithMessage(ValidatorTranform.ValidValue("email"));

            RuleFor(x => x.AcademicTitle)
                .Must(academicTitle => string.IsNullOrEmpty(academicTitle) || academicTitle == CommonTranform.bachelor || academicTitle == CommonTranform.engineer ||
                academicTitle == CommonTranform.postgraduate || academicTitle == CommonTranform.master || academicTitle == CommonTranform.doctorate)
                .WithMessage(ValidatorTranform.Must("academicTitle", CommonTranform.GetListAcademicTitle()));

            RuleFor(x => x.Degree)
                .Must((dto, degree) =>
                    string.IsNullOrEmpty(dto.AcademicTitle) || dto.AcademicTitle != CommonTranform.doctorate
                    ? string.IsNullOrEmpty(degree)
                    : degree == CommonTranform.associateProfessor || degree == CommonTranform.professor || string.IsNullOrWhiteSpace(degree))
                .WithMessage(ValidatorTranform.MustWhen("degree", CommonTranform.GetListDegree(), "academicTitle", CommonTranform.doctorate));
        }    
    }
}
