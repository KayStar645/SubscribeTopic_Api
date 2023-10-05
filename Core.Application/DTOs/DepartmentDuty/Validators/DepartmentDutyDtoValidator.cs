using Core.Application.Contracts.Persistence;
using Core.Application.Custom;
using Core.Application.Transform;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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
                    var exists = await _unitOfWork.Repository<TeacherEntity>().FirstOrDefaultAsync(x=>x.Id == id && x.DepartmentId == departmentId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("teacherId"));

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
                .Must(timeEnd => CustomValidator.IsAfterDay(timeEnd, start))
                .WithMessage(ValidatorTranform.GreaterThanDay("timeEnd", start));

            RuleFor(x => x.Image)
                .Must(image => string.IsNullOrEmpty(image) || Uri.TryCreate(image, UriKind.Absolute, out _))
                .WithMessage(ValidatorTranform.MustUrl("image"));

            

        }
    }
}
