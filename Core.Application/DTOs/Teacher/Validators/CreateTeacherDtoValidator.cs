using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class CreateTeacherDtoValidator : AbstractValidator<CreateTeacherDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTeacherDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new TeacherDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
               .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
               .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
               .MustAsync(async (internalCode, token) =>
               {
                   var teacher = await _unitOfWork.Repository<TeacherEntity>()
                       .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                   return teacher == null;
               })
               .WithMessage(internalCode => ValidatorTranform.Exists("internalCode"));
        }
    }
}
