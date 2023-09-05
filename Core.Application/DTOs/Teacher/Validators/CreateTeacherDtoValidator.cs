using FluentValidation;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class CreateTeacherDtoValidator : AbstractValidator<CreateTeacherDto>
    {

        public CreateTeacherDtoValidator()
        {

            Include(new TeacherDtoValidator());
        }
    }
}
