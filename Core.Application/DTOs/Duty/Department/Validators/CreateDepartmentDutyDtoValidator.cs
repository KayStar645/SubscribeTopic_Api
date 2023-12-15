using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Transform;
using FluentValidation;
using DutyEntity = Core.Domain.Entities.Duty;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.Duty.Validators
{
    public class CreateDepartmentDutyDtoValidator : AbstractValidator<CreateDepartmentDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateDepartmentDutyDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new DutyDtoValidator(_unitOfWork));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190))
            .MustAsync(async (name, token) =>
            {
                var exists = await _unitOfWork.Repository<DutyEntity>()
                                    .FirstOrDefaultAsync(x => x.Name == name);
                return exists == null;
            }).WithMessage(ValidatorTransform.Exists("name"));

            RuleFor(x => x.TeacherId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<TeacherEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("teacherId", "teacher"));

            RuleFor(x => x.DutyId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<DutyEntity>()
                            .FirstOrDefaultAsync(x => x.Id == id && x.Type == DutyEntity.TYPE_FACULTY);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("dutyId", "duty"));
        }
    }
}
