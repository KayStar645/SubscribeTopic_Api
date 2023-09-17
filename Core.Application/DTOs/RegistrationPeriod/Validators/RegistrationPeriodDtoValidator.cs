using Core.Application.Contracts.Persistence;
using Core.Application.Custom;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.RegistrationPeriod.Validators
{
    public class RegistrationPeriodDtoValidator : AbstractValidator<IRegistrationPeriodDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RegistrationPeriodDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Phase)
                .NotEmpty().WithMessage(ValidatorTranform.Required("phase"))
                .GreaterThan(0).WithMessage(ValidatorTranform.GreaterThanOrEqualTo("phase", 1));

            RuleFor(x => x.Semester)
                .NotEmpty().WithMessage(ValidatorTranform.Required("semester"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("name", 50));

            RuleFor(x => x.TimeStart)
                .Must(timestart => string.IsNullOrEmpty(timestart.ToString()) || CustomValidator.IsAfterToday(timestart))
                .WithMessage(ValidatorTranform.GreaterThanToday("timestart"));

            RuleFor(x => x.TimeEnd)
                .Must(timeEnd => string.IsNullOrEmpty(timeEnd.ToString()) || CustomValidator.IsAfterToday(timeEnd))
                .WithMessage(ValidatorTranform.GreaterThanToday("timestart"));

            //Chưa xử lý được timeEnd phải sau timeStart
        }
    }
}
