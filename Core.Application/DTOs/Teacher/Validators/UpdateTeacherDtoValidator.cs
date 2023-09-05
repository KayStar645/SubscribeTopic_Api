using FluentValidation;
using Core.Application.Transform;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class UpdateTeacherDtoValidator : AbstractValidator<UpdateTeacherDto>
    {
        public UpdateTeacherDtoValidator()
        {
            Include(new TeacherDtoValidator());

            RuleFor(p => p.Id).NotNull().WithMessage(ValidatorTranform.Required("Id"));
        }
    }
}
