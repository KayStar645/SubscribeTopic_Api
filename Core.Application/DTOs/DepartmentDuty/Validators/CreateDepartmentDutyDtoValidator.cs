using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using DepartmentDutyEntity = Core.Domain.Entities.DepartmentDuty;

namespace Core.Application.DTOs.DepartmentDuty.Validators
{
    public class CreateDepartmentDutyDtoValidator : AbstractValidator<CreateDepartmentDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDepartmentDutyDtoValidator(IUnitOfWork unitOfWork, DateTime start)
        {
            _unitOfWork = unitOfWork;

            Include(new DepartmentDutyDtoValidator(_unitOfWork, start));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentDutyEntity>()
                        .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190));


        }
    }
}
