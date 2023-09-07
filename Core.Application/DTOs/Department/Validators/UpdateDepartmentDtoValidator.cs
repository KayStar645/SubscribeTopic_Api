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

            /*
             RuleFor(p => p.Id).NotEmpty().WithMessage(ValidatorTranform.Required("Id"))
                .MustAsync(async (id, token) =>
                {
                    var teacherExists = await _unitOfWork.Repository<Teacher>().GetByIdAsync(id);
                    return teacherExists != null;
                })
                .WithMessage("{PropertyName} does exist.");
             */
        }
    }
}
