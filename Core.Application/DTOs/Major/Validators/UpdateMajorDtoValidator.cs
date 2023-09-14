using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Major.Validators
{
    public class UpdateMajorDtoValidator : AbstractValidator<IMajorDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMajorDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new MajorDtoValidator(_unitOfWork));
        }
    }
}
