using Core.Application.Contracts.Persistence;
using Core.Application.Custom;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;

namespace Core.Application.DTOs.RegistrationPeriod.Validators
{
    public class RegistrationPeriodDtoValidator : AbstractValidator<IRegistrationPeriodDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RegistrationPeriodDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Phase)
                .NotEmpty().WithMessage(ValidatorTranform.Required("phase"))
                .GreaterThan(0).WithMessage(ValidatorTranform.GreaterThanOrEqualTo("phase", 1));

            RuleFor(x => x.Semester)
                .Must(semester => semester == CommonTranform.semester1 || semester == CommonTranform.semester2 || semester == CommonTranform.semester3)
                .WithMessage(ValidatorTranform.Must("semester", CommonTranform.GetListSemester()));

            RuleFor(x => x.Year)
                .NotEmpty().WithMessage(ValidatorTranform.Required("year"));

            RuleFor(x => x.TimeStart)
                .Must(timestart => string.IsNullOrEmpty(timestart.ToString()) || CustomValidator.IsAfterToday(timestart))
                .WithMessage(ValidatorTranform.GreaterThanToday("timestart"));

            //Chưa xử lý được timeEnd phải sau timeStart
            RuleFor(x => x.TimeEnd)
                .Must(timeEnd => string.IsNullOrEmpty(timeEnd.ToString()) || CustomValidator.IsAfterToday(timeEnd))
                .WithMessage(ValidatorTranform.GreaterThanToday("timestart"));

            RuleFor(x => x.FacultyId)
                .MustAsync(async (id, token) =>
                {
                    var existsFaculty = await _unitOfWork.Repository<FacultyEntity>().GetByIdAsync(id);
                    return existsFaculty != null;
                })
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("facultyId", "faculty"));
        }
    }
}
