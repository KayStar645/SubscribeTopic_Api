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
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                                        .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190))
                .MustAsync(async (name, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                                        .FirstOrDefaultAsync(x => x.Name == name);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("name"));
        }
    }
}
