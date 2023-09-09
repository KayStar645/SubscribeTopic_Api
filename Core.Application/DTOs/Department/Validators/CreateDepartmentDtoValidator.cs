using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Department.Validators
{
    public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDepartmentDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            Include(new DepartmentDtoValidator(_unitOfWork));
        }
    }
}
