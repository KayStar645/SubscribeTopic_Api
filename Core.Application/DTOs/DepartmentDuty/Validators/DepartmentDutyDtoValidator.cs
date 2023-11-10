using Core.Application.Contracts.Persistence;
using Core.Application.Services;
using Core.Application.Transform;
using FluentValidation;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.DepartmentDuty.Validators
{
    public class DepartmentDutyDtoValidator : AbstractValidator<IDepartmentDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentDutyDtoValidator(IUnitOfWork unitOfWork, int? departmentId, DateTime start)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.TeacherId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<TeacherEntity>().FirstOrDefaultAsync(x => x.Id == id && x.DepartmentId == departmentId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("teacherId"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190));

            RuleFor(x => x.NumberOfThesis)
                .NotEmpty().WithMessage(ValidatorTransform.Required("numberOfThesis"))
                .GreaterThanOrEqualTo(1).WithMessage(ValidatorTransform.GreaterThanOrEqualTo("numberOfThesis", 1));

            RuleFor(x => x.TimeStart)
                .Must(timeStart => CustomValidator.IsEqualOrAfterDay(timeStart, DateTime.Now))
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
