using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using RegistrationPeriodEntity = Core.Domain.Entities.RegistrationPeriod;
using StudentEntity = Core.Domain.Entities.Student;

namespace Core.Application.DTOs.StudentJoin.Validators
{
    public class CreateStudentJoinDtoValidator : AbstractValidator<CreateStudentJoinDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentJoinDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.studentIds)
            .MustAsync(async (ids, token) =>
            {
                foreach (var id in ids)
                {
                    var student = await _unitOfWork.Repository<StudentEntity>().GetByIdAsync(id);
                    if (student == null)
                    {
                        return false;
                    }
                }
                return true;
            })
            .WithMessage(id => ValidatorTransform.NotExistsValueInTable("studentId", "students"));

            RuleFor(x => x.registrationPeriodId)
            .MustAsync(async (id, token) =>
            {
                var exists = await _unitOfWork.Repository<RegistrationPeriodEntity>().GetByIdAsync(id);
                return exists != null;
            })
            .WithMessage(id => ValidatorTransform.NotExistsValueInTable("registrationPeriodId", "registrationPeriods"));
        }
    }
}
