using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;

namespace Core.Application.DTOs.Faculty.Validators
{
    public class CreateFacultyDtoValidator : AbstractValidator<CreateFacultyDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFacultyDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            Include(new FacultyDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var faculty = await _unitOfWork.Repository<FacultyEntity>()
                                        .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return faculty == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));
        }
    }
}
