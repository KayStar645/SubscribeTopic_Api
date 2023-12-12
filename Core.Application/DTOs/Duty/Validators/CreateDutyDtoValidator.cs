using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;
using DutyEntity = Core.Domain.Entities.Duty;
using PeriodEnity = Core.Domain.Entities.RegistrationPeriod;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.Duty.Validators
{
    public class CreateDutyDtoValidator : AbstractValidator<CreateDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateDutyDtoValidator(IUnitOfWork unitOfWork, string? pType)
        {
            _unitOfWork = unitOfWork;

            Include(new DutyDtoValidator(_unitOfWork));

            RuleFor(x => x.Type)
                .Must(type => DutyEntity.GetType().Contains(type))
                .WithMessage(ValidatorTransform.Must("type", DutyEntity.GetType()));

            if (pType == DutyEntity.TYPE_FACULTY)
            {
                RuleFor(x => x.DepartmentId)
                 .MustAsync(async (id, token) =>
                 {
                     var exists = await _unitOfWork.Repository<DepartmentEntity>().GetByIdAsync(id);
                     return exists != null;
                 })
                 .WithMessage(id => ValidatorTransform.NotExistsValueInTable("departmentId", "department"));
            }
            else if(pType == DutyEntity.TYPE_DEPARTMENT)
            {
                RuleFor(x => x.TeacherId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<TeacherEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("teacherId", "teacher"));
            }    

        }
    }
}
