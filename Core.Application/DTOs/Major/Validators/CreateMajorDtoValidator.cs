using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using MajorEntity = Core.Domain.Entities.Major;

namespace Core.Application.DTOs.Major.Validators
{
    public class CreateMajorDtoValidator : AbstractValidator<IMajorDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateMajorDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            Include(new MajorDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTransform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTransform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var major = await _unitOfWork.Repository<MajorEntity>()
                                      .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return major == null;
                }).WithMessage(ValidatorTransform.Exists("internalCode"));
        }
    }
}
