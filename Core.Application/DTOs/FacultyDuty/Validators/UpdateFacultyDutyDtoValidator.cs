using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using FacultyDutyEntity = Core.Domain.Entities.FacultyDuty;
using FacultyEntity = Core.Domain.Entities.Faculty;

namespace Core.Application.DTOs.FacultyDuty.Validators
{
    public class UpdateFacultyDutyDtoValidator : AbstractValidator<UpdateFacultyDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFacultyDutyDtoValidator(IUnitOfWork unitOfWork, int currentId, DateTime start)
        {
            _unitOfWork = unitOfWork;

            Include(new FacultyDutyDtoValidator(_unitOfWork, start));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTranform.Required("id"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyDutyEntity>()
                        .FirstOrDefaultAsync(x => x.Id != currentId && x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));




        }
    }
}
