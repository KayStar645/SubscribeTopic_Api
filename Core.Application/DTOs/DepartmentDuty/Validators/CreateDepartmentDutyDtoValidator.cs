using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;
using DepartmentDutyEntity = Core.Domain.Entities.DepartmentDuty;

namespace Core.Application.DTOs.DepartmentDuty.Validators
{
    public class CreateDepartmentDutyDtoValidator : AbstractValidator<CreateDepartmentDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDepartmentDutyDtoValidator(IUnitOfWork unitOfWork, int? departmentId, DateTime start)
        {
            _unitOfWork = unitOfWork;

            Include(new DepartmentDutyDtoValidator(_unitOfWork, departmentId, start));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentDutyEntity>()
                        .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));

            RuleFor(x => x.DepartmentId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("departmentId", "departments"));

        }
    }
}
