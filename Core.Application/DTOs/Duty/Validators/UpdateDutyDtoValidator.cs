using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Duty.Validators
{
    public class UpdateDutyDtoValidator : AbstractValidator<UpdateDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateDutyDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new DutyDtoValidator(_unitOfWork));
        }
    }
}
