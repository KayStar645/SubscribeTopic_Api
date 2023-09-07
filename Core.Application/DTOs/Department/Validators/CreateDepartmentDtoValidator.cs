using FluentValidation;

namespace Core.Application.DTOs.Department.Validators
{
    public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
    {
        public CreateDepartmentDtoValidator() 
        {
            Include(new DepartmentDtoValidator());
        }
    }
}
