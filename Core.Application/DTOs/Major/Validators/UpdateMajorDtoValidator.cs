using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using MajorEntity = Core.Domain.Entities.Major;

namespace Core.Application.DTOs.Major.Validators
{
    public class UpdateMajorDtoValidator : AbstractValidator<IMajorDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMajorDtoValidator(IUnitOfWork unitOfWork, int currentMajorId)
        {
            _unitOfWork = unitOfWork;

            Include(new MajorDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var major = await _unitOfWork.Repository<MajorEntity>()
                                      .FirstOrDefaultAsync(x => x.Id != currentMajorId && x.InternalCode == internalCode);
                    return major == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));
        }
    }
}
