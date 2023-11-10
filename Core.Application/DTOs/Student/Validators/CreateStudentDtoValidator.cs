using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using StudentEntity = Core.Domain.Entities.Student;

namespace Core.Application.DTOs.Student.Validators
{
    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new StudentDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTransform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTransform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var student = await _unitOfWork.Repository<StudentEntity>()
                                        .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return student == null;
                }).WithMessage(ValidatorTransform.Exists("internalCode"));
        }
    }
}
