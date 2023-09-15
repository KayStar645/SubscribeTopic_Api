using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Student.Validators
{
    public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new StudentDtoValidator(_unitOfWork));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTranform.Required("id"));
        }
    }
}
