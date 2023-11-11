using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using FacultyDutyEntity = Core.Domain.Entities.FacultyDuty;
using FacultyEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.DTOs.FacultyDuty.Validators
{
    public class UpdateFacultyDutyDtoValidator : AbstractValidator<UpdateFacultyDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFacultyDutyDtoValidator(IUnitOfWork unitOfWork, int? currentId, int? facultyId, DateTime start)
        {
            _unitOfWork = unitOfWork;

            Include(new FacultyDutyDtoValidator(_unitOfWork, facultyId, start));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTransform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTransform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyDutyEntity>()
                        .FirstOrDefaultAsync(x => x.Id != currentId && x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("internalCode"));




        }
    }
}
