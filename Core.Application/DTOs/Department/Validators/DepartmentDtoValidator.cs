using Core.Application.Contracts.Persistence;
using Core.Application.Services;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculties;

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
                    var exists = await _unitOfWork.Repository<FacultyEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("facultyId", "faculties"));

            RuleFor(x => x.PhoneNumber)
                .Must(phoneNumber => string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length == 10)
                .WithMessage(ValidatorTransform.Length("phoneNumber", 10));

            RuleFor(x => x.Email)
                .Must(email => string.IsNullOrEmpty(email) || CustomValidator.BeValidEmail(email))
                .WithMessage(ValidatorTransform.ValidValue("email"));
        }
    }
}
