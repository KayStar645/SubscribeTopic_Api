using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.RegistrationPeriod.Validators
{
    public class CreateRegistrationPeriodDtoValidator : AbstractValidator<CreateRegistrationPeriodDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRegistrationPeriodDtoValidator(IUnitOfWork unitOfWork, DateTime start)
        {
            _unitOfWork = unitOfWork;

            Include(new RegistrationPeriodDtoValidator(_unitOfWork, start));
        }
    }
}
