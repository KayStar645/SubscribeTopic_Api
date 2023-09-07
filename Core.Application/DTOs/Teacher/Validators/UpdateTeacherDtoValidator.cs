using FluentValidation;
using Core.Application.Transform;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class UpdateTeacherDtoValidator : AbstractValidator<UpdateTeacherDto>
    {
        public UpdateTeacherDtoValidator()
        {
            Include(new TeacherDtoValidator());

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTranform.Required("id"));
        }
    }
}
