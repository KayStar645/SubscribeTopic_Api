using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Student.Validators
{
    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new StudentDtoValidator(_unitOfWork));
        }
    }
}
