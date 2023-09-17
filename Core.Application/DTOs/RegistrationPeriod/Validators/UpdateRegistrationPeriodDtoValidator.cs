using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.RegistrationPeriod.Validators
{
    public class UpdateRegistrationPeriodDtoValidator : AbstractValidator<UpdateRegistrationPeriodDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRegistrationPeriodDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new RegistrationPeriodDtoValidator(_unitOfWork));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTranform.Required("id"));
        }
    }
}
