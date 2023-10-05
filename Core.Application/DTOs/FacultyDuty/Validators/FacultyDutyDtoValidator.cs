using Core.Application.Contracts.Persistence;
using Core.Application.Custom;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;

namespace Core.Application.DTOs.FacultyDuty.Validators
{
    public class FacultyDutyDtoValidator : AbstractValidator<IFacultyDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacultyDutyDtoValidator(IUnitOfWork unitOfWork, int? facultyId, DateTime start)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.DepartmentId)
                .MustAsync(async (id, token) =>
                {
                    var departmentExists = await _unitOfWork.Repository<DepartmentEntity>().FirstOrDefaultAsync(x => x.Id == id && x.FacultyId == facultyId);
                    return departmentExists != null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("departmentId"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190));

            RuleFor(x => x.NumberOfThesis)
                .NotEmpty().WithMessage(ValidatorTranform.Required("numberOfThesis"))
                .GreaterThanOrEqualTo(1).WithMessage(ValidatorTranform.GreaterThanOrEqualTo("numberOfThesis", 1));

            RuleFor(x => x.TimeStart)
                .Must(timeStart => CustomValidator.IsEqualOrAfterDay(timeStart, DateTime.Now))
                .WithMessage(ValidatorTranform.GreaterEqualOrThanDay("timestart", DateTime.Now));

            RuleFor(x => x.TimeEnd)
                .NotEmpty().WithMessage(ValidatorTranform.Required("timeEnd"))
                .Must(timeEnd => CustomValidator.IsAfterDay(timeEnd, start))
                .WithMessage(ValidatorTranform.GreaterThanDay("timeEnd", start));

            RuleFor(x => x.Image)
                .Must(image => string.IsNullOrEmpty(image) || Uri.TryCreate(image, UriKind.Absolute, out _))
                .WithMessage(ValidatorTranform.MustUrl("image"));


        }
    }
}
