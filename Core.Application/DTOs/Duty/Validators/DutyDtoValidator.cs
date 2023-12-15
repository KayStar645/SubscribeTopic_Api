using Core.Application.Contracts.Persistence;
using Core.Application.Services;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Duty.Validators
{
    public class DutyDtoValidator : AbstractValidator<IDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DutyDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Content)
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 6000));

            RuleFor(x => x.NumberOfThesis)
                .NotEmpty().WithMessage(ValidatorTransform.Required("numberOfThesis"))
                .GreaterThanOrEqualTo(1).WithMessage(ValidatorTransform.GreaterThanOrEqualTo("numberOfThesis", 1));

            RuleFor(x => x.TimeEnd)
                .Must(timeEnd => CustomValidator.IsAfterDay(timeEnd, DateTime.Now))
                .WithMessage(ValidatorTransform.GreaterThanDay("timeEnd", DateTime.Now));

            RuleFor(x => x.Files)
                .Must(images =>
                {
                    if (images == null)
                        return true;
                    foreach (var image in images)
                    {
                        if (!string.IsNullOrEmpty(image) && !Uri.TryCreate(image, UriKind.Absolute, out _))
                            return false;
                    }

                    return true;
                })
                .WithMessage(ValidatorTransform.MustUrls("files"));
        }
    }
}
