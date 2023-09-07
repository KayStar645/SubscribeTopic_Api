using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class CreateTeacherDtoValidator : AbstractValidator<CreateTeacherDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTeacherDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new TeacherDtoValidator(_unitOfWork));
        }
    }
}
