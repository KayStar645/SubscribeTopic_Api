using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using DepartmentDutyEntity = Core.Domain.Entities.DepartmentDuty;

namespace Core.Application.DTOs.DepartmentDuty.Validators
{
    public class UpdateDepartmentDutyDtoValidator : AbstractValidator<UpdateDepartmentDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentDutyDtoValidator(IUnitOfWork unitOfWork, int currentId, DateTime start)
        {
            _unitOfWork = unitOfWork;

            Include(new DepartmentDutyDtoValidator(_unitOfWork, start));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTranform.Required("id"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentDutyEntity>()
                        .FirstOrDefaultAsync(x => x.Id != currentId && x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));


        }
    }
}
