using Core.Application.Contracts.Persistence;
using Core.Application.Services;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.RegistrationPeriod.Validators
{
    public class CreateRegistrationPeriodDtoValidator : AbstractValidator<CreateRegistrationPeriodDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRegistrationPeriodDtoValidator(IUnitOfWork unitOfWork, DateTime start)
        {
            _unitOfWork = unitOfWork;

            Include(new RegistrationPeriodDtoValidator(_unitOfWork));

            RuleFor(x => x.TimeStart)
                .Must(timeStart => CustomValidator.IsEqualOrAfterDay(timeStart, DateTime.Now))
                .WithMessage(ValidatorTransform.GreaterEqualOrThanDay("timestart", DateTime.Now));

            RuleFor(x => x.TimeEnd)
                .Must(timeEnd => CustomValidator.IsAfterDay(timeEnd, start))
                .WithMessage(ValidatorTransform.GreaterThanDay("timeEnd", start));
        }
    }
}
