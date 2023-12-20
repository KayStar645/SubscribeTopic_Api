using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using CouncilEntity = Core.Domain.Entities.Council;

namespace Core.Application.DTOs.Council.Validators
{
    public class UpdateCouncilDtoValidator : AbstractValidator<ICouncilDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCouncilDtoValidator(IUnitOfWork unitOfWork, int? currentId)
        {
            _unitOfWork = unitOfWork;

            Include(new CouncilDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTransform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTransform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<CouncilEntity>()
                        .FirstOrDefaultAsync(x => x.Id != currentId && x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("internalCode"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190))
                .MustAsync(async (name, token) =>
                {
                    var exists = await _unitOfWork.Repository<CouncilEntity>()
                        .FirstOrDefaultAsync(x => x.Id != currentId && x.Name == name);
                    return exists == null;
                }).WithMessage(ValidatorTransform.Exists("name"));
        }
    }
}
