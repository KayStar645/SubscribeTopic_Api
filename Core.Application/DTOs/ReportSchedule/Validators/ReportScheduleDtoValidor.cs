using Core.Application.Contracts.Persistence;
using Core.Application.Services;
using Core.Application.Transform;
using FluentValidation;
using ReportScheduleEntity = Core.Domain.Entities.ReportSchedule;

namespace Core.Application.DTOs.ReportSchedule.Validators
{
    public class ReportScheduleDtoValidor : AbstractValidator<IReportScheduleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportScheduleDtoValidor(IUnitOfWork unitOfWork, DateTime start)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.TimeStart)
                .Must(timeStart => timeStart == null || CustomValidator.IsEqualOrAfterDay(timeStart, DateTime.Now))
                .WithMessage(ValidatorTransform.GreaterEqualOrThanDay("timestart", DateTime.Now));

            RuleFor(x => x.TimeEnd)
                .NotEmpty().WithMessage(ValidatorTransform.Required("timeEnd"))
                .Must(timeEnd => CustomValidator.IsAfterDay(timeEnd, start))
                .WithMessage(ValidatorTransform.GreaterThanDay("timeEnd", start));

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage(ValidatorTransform.Required("location"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("location", 500));

            RuleFor(x => x.Type)
                .Must(type => ReportScheduleEntity.GetType().Any(x => x.Equals(type)))
                .WithMessage(ValidatorTransform.Must("type", ReportScheduleEntity.GetType()));
        }
    }
}
