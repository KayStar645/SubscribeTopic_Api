using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;
using FacultyDutyEntity = Core.Domain.Entities.FacultyDuty;

namespace Core.Application.DTOs.FacultyDuty.Validators
{
    public class CreateFacultyDutyDtoValidator : AbstractValidator<CreateFacultyDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFacultyDutyDtoValidator(IUnitOfWork unitOfWork, int? facultyId, DateTime start)
        {
            _unitOfWork = unitOfWork;

            Include(new FacultyDutyDtoValidator(_unitOfWork, facultyId, start));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyDutyEntity>()
                        .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));

            RuleFor(x => x.FacultyId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("facultyId", "faculties"));

        }
    }
}
