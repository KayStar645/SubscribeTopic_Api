using FluentValidation;
using Core.Application.Transform;
using Core.Application.Contracts.Persistence;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class UpdateTeacherDtoValidator : AbstractValidator<UpdateTeacherDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTeacherDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new TeacherDtoValidator(_unitOfWork));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTranform.Required("id"));
        }
    }
}
