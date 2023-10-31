using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;

namespace Core.Application.DTOs.Department.Validators
{
    public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDepartmentDtoValidator(IUnitOfWork unitOfWork, int currentFacultyId) 
        {
            _unitOfWork = unitOfWork;

            Include(new DepartmentDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentEntity>()
                        .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190))
                .MustAsync(async (name, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentEntity>()
                        .FirstOrDefaultAsync(x => x.Name == name && x.FacultyId == currentFacultyId);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("name"));
        }
    }
}
