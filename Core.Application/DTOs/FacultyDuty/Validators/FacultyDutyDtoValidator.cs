using Core.Application.Contracts.Persistence;
using Core.Application.Custom;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;
using DepartmentEntity = Core.Domain.Entities.Department;

namespace Core.Application.DTOs.FacultyDuty.Validators
{
    public class FacultyDutyDtoValidator : AbstractValidator<IFacultyDutyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacultyDutyDtoValidator(IUnitOfWork unitOfWork, DateTime start)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.FacultyId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("facultyId", "facultys"));

            RuleFor(x => x.DepartmentId)
                .MustAsync(async (id, token) =>
                {
                    var departmentExists = await _unitOfWork.Repository<DepartmentEntity>().GetByIdAsync(id);
                    return departmentExists != null;
                })
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("departmentId", "departments"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190));

            RuleFor(x => x.NumberOfThesis)
                .NotEmpty().WithMessage(ValidatorTranform.Required("numberOfThesis"))
                .GreaterThanOrEqualTo(1).WithMessage(ValidatorTranform.GreaterThanOrEqualTo("numberOfThesis", 1));

            RuleFor(x => x.TimeStart)
                .Must(timeStart => CustomValidator.IsEqualOrAfterDay(timeStart, DateTime.Now))
                .WithMessage(ValidatorTranform.GreaterEqualOrThanDay("timestart", DateTime.Now));

            RuleFor(x => x.TimeEnd)
                .Must(timeEnd => CustomValidator.IsAfterDay(timeEnd, start))
                .WithMessage(ValidatorTranform.GreaterThanDay("timeEnd", start));

            RuleFor(x => x.Image)
                .Must(image => string.IsNullOrEmpty(image) || Uri.TryCreate(image, UriKind.Absolute, out _))
                .WithMessage(ValidatorTranform.MustUrl("image"));


        }
    }
}
