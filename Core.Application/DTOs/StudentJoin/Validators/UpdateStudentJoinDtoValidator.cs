using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.StudentJoin.Validators
{
    public class UpdateStudentJoinDtoValidator : AbstractValidator<IStudentJoinDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentJoinDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new StudentJoinDtoValidator(_unitOfWork));
        }
    }
}
