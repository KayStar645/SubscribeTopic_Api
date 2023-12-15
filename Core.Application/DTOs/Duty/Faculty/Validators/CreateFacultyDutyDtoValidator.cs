using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;
using DutyEntity = Core.Domain.Entities.Duty;

namespace Core.Application.DTOs.Duty.Validators
{
    public class CreateFacultyDutyDtoValidator : AbstractValidator<CreateFacultyDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateFacultyDutyDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new DutyDtoValidator(_unitOfWork));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190))
            .MustAsync(async (name, token) =>
            {
                var exists = await _unitOfWork.Repository<DutyEntity>()
                                    .FirstOrDefaultAsync(x => x.Name == name);
                return exists == null;
            }).WithMessage(ValidatorTransform.Exists("name"));

            RuleFor(x => x.DepartmentId)
                 .MustAsync(async (id, token) =>
                 {
                     var exists = await _unitOfWork.Repository<DepartmentEntity>().GetByIdAsync(id);
                     return exists != null;
                 })
                 .WithMessage(id => ValidatorTransform.NotExistsValueInTable("departmentId", "department"));
        }
    }
}
