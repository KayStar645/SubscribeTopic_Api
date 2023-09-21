using FluentValidation;
using Core.Application.Transform;
using Core.Application.Contracts.Persistence;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class UpdateTeacherDtoValidator : AbstractValidator<UpdateTeacherDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTeacherDtoValidator(IUnitOfWork unitOfWork, int currentTeacherId)
        {
            _unitOfWork = unitOfWork;

            Include(new TeacherDtoValidator(_unitOfWork));

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTranform.Required("id"));

            RuleFor(x => x.InternalCode)
               .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
               .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
               .MustAsync(async (internalCode, token) =>
               {
                   var teacher = await _unitOfWork.Repository<TeacherEntity>()
                       .FirstOrDefaultAsync(x => x.Id != currentTeacherId && x.InternalCode == internalCode);
                   return teacher == null;
               })
               .WithMessage(internalCode => ValidatorTranform.Exists("internalCode"));
        }
    }
}
