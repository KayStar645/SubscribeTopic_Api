using Core.Application.Contracts.Persistence;
using Core.Application.Custom;
using Core.Application.Transform;
using FluentValidation;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.Faculty.Validators
{
    public class FacultyDtoValidator : AbstractValidator<IFacultyDto>
    {
        private readonly IUnitOfWork unitOfWork;

        public FacultyDtoValidator(IUnitOfWork _unitOfWork) 
        {
            unitOfWork = _unitOfWork;

            RuleFor(x => x.Dean_TeacherId)
                .MustAsync(async (id, token) =>
                {
                    var HeadDepartmentExists = await _unitOfWork.Repository<TeacherEntity>().GetByIdAsync(id);
                    return HeadDepartmentExists != null;
                })
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("dean_TeacherId", "teachers"));

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
