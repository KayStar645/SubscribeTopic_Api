using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DepartmentEntity = Core.Domain.Entities.Department;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.Department.Validators
{
    public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentDtoValidator(IUnitOfWork unitOfWork, int? currentId, int currentFacultyId)
        {
            _unitOfWork = unitOfWork;

            Include(new DepartmentDtoValidator(_unitOfWork));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTransform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTransform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentEntity>()
                        .FirstOrDefaultAsync(x => x.Id != currentId && x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190))
                .MustAsync(async (name, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentEntity>()
                        .FirstOrDefaultAsync(x => x.Id != currentId && x.Name == name && x.FacultyId == currentFacultyId);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("name"));

            RuleFor(x => x.HeadDepartment_TeacherId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<TeacherEntity>().GetByIdInclude(id)
                                           .Where(x => x.DepartmentId == currentId)
                                           .FirstOrDefaultAsync();
                    return exists != null || id == null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("headDepartment_TeacherId"));
        }
    }
}
