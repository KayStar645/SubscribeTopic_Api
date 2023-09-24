using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;

namespace Core.Application.DTOs.Department.Validators
{
    public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentDtoValidator(IUnitOfWork unitOfWork, int currentId, int currentFacultyId)
        {
            _unitOfWork = unitOfWork;

            Include(new DepartmentDtoValidator(_unitOfWork));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTranform.Required("id"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentEntity>()
                        .FirstOrDefaultAsync(x => x.Id != currentId && x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190))
                .MustAsync(async (name, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentEntity>()
                        .FirstOrDefaultAsync(x => x.Id != currentId && x.Name == name && x.FacultyId == currentFacultyId);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("name"));
        }
    }
}
