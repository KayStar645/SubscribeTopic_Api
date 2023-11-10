using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using static System.Net.Mime.MediaTypeNames;
using DepartmentEntity = Core.Domain.Entities.Department;
using Core.Application.Services;

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
                .WithMessage(id => ValidatorTransform.MustIn("departmentId"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190));

            RuleFor(x => x.NumberOfThesis)
                .NotEmpty().WithMessage(ValidatorTransform.Required("numberOfThesis"))
                .GreaterThanOrEqualTo(1).WithMessage(ValidatorTransform.GreaterThanOrEqualTo("numberOfThesis", 1));
            RuleFor(x => x.TimeStart)
                .Must(timeStart => timeStart == null || CustomValidator.IsEqualOrAfterDay(timeStart, DateTime.Now))
                .WithMessage(ValidatorTransform.GreaterEqualOrThanDay("timestart", DateTime.Now));

            RuleFor(x => x.TimeEnd)
                .NotEmpty().WithMessage(ValidatorTransform.Required("timeEnd"))
                .Must(timeEnd => CustomValidator.IsAfterDay(timeEnd, start))
                .WithMessage(ValidatorTransform.GreaterThanDay("timeEnd", start));

            RuleFor(x => x.Image)
                .Must(image => string.IsNullOrEmpty(image) || Uri.TryCreate(image, UriKind.Absolute, out _))
                .WithMessage(ValidatorTransform.MustUrl("image"));

            RuleFor(x => x.File)
                .Must(file => string.IsNullOrEmpty(file) || CustomValidator.IsValidFile(file))
                .WithMessage(ValidatorTransform.MustFile("file"));
        }
    }
}
