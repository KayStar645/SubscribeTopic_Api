using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Faculty.Validators
{
    public class UpdateFacultyDtoValidator : AbstractValidator<UpdateFacultyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFacultyDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new FacultyDtoValidator(_unitOfWork));
        }
    }
}
