using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Department.Validators
{
    public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepartmentDtoValidator() 
        {

            Include(new DepartmentDtoValidator());

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTranform.Required("id"));
        }
    }
}
