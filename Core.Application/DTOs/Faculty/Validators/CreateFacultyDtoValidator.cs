using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Faculty.Validators
{
    public class CreateFacultyDtoValidator : AbstractValidator<CreateFacultyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFacultyDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            Include(new FacultyDtoValidator(_unitOfWork));
        }
    }
}
