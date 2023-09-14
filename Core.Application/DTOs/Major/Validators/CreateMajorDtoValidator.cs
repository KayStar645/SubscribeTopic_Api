using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Major.Validators
{
    public class CreateMajorDtoValidator : AbstractValidator<IMajorDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateMajorDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            Include(new MajorDtoValidator(_unitOfWork));
        }
    }
}
