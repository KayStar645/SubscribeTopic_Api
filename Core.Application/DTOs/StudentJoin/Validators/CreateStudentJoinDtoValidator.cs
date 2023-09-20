using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.StudentJoin.Validators
{
    public class CreateStudentJoinDtoValidator : AbstractValidator<IStudentJoinDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentJoinDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new StudentJoinDtoValidator(_unitOfWork));
        }
    }
}
