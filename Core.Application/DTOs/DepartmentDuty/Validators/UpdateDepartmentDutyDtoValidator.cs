using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using DepartmentDutyEntity = Core.Domain.Entities.DepartmentDuty;

namespace Core.Application.DTOs.DepartmentDuty.Validators
{
    public class UpdateDepartmentDutyDtoValidator : AbstractValidator<UpdateDepartmentDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentDutyDtoValidator(IUnitOfWork unitOfWork, int? currentId, int? departmentId, DateTime start)
        {
            _unitOfWork = unitOfWork;

            Include(new DepartmentDutyDtoValidator(_unitOfWork, departmentId, start));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTransform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTransform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentDutyEntity>()
                        .FirstOrDefaultAsync(x => x.Id != currentId && x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("internalCode"));


        }
    }
}
