using Core.Application.Contracts.Persistence;
using Core.Application.Interfaces.Repositories;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.RegistrationPeriod.Validators
{
    public class UpdateRegistrationPeriodDtoValidator : AbstractValidator<UpdateRegistrationPeriodDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRegistrationPeriodDtoValidator(IUnitOfWork unitOfWork, DateTime start)
        {
            _unitOfWork = unitOfWork;

            Include(new RegistrationPeriodDtoValidator(_unitOfWork, start));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));
        }
    }
}
