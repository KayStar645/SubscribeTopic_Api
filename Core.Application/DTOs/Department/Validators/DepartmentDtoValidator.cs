using Core.Application.Contracts.Persistence;
using Core.Application.Custom;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;

namespace Core.Application.DTOs.Department.Validators
{
    public class DepartmentDtoValidator : AbstractValidator<IDepartmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.FacultyId)
                .MustAsync(async (id, token) =>
                {
                    var facultyExists = await _unitOfWork.Repository<FacultyEntity>().GetByIdAsync(id);
                    return facultyExists != null || id == null;
                })
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("departmentId", "departments"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"));

            RuleFor(x => x.PhoneNumber)
                .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length == 10)
                .WithMessage(ValidatorTranform.Length("phoneNumber", 10));

            RuleFor(x => x.Email)
                .Must(email => string.IsNullOrEmpty(email) || CustomValidator.BeValidEmail(email))
                .WithMessage(ValidatorTranform.ValidValue("email"));
        }
    }
}
