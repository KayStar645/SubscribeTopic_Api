using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using StudentEntity = Core.Domain.Entities.Student;
using RegistrationPeriodEntity = Core.Domain.Entities.RegistrationPeriod;

namespace Core.Application.DTOs.StudentJoin.Validators
{
    public class StudentJoinDtoValidator : AbstractValidator<IStudentJoinDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentJoinDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.studentId)
            .MustAsync(async (id, token) =>
            {
                    var exists = await _unitOfWork.Repository<StudentEntity>().GetByIdAsync(id);
                    return exists != null;
            })
            .WithMessage(id => ValidatorTranform.NotExistsValueInTable("studentId", "students"));

            RuleFor(x => x.registrationPeriodId)
            .MustAsync(async (id, token) =>
            {
                var exists = await _unitOfWork.Repository<RegistrationPeriodEntity>().GetByIdAsync(id);
                return exists != null;
            })
            .WithMessage(id => ValidatorTranform.NotExistsValueInTable("registrationPeriodId", "registrationPeriods"));
        }
    }
}
