using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using StudentEntity = Core.Domain.Entities.Student;

namespace Core.Application.DTOs.Student.Validators
{
    public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentDtoValidator(IUnitOfWork unitOfWork, int currentStudentId)
        {
            _unitOfWork = unitOfWork;

            Include(new StudentDtoValidator(_unitOfWork));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTranform.Required("id"));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var student = await _unitOfWork.Repository<StudentEntity>()
                                        .FirstOrDefaultAsync(x => x.Id != currentStudentId && x.InternalCode == internalCode);
                    return student == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));
        }
    }
}
