using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using IndustryEntity = Core.Domain.Entities.Industry;

namespace Core.Application.DTOs.Industry.Validators
{
    public class CreateIndustryDtoValidator : AbstractValidator<CreateIndustryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateIndustryDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            Include(new IndustryDtoValidator(_unitOfWork));

            RuleFor(x => x.InternalCode)
                .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50))
                .MustAsync(async (internalCode, token) =>
                {
                    var exists = await _unitOfWork.Repository<IndustryEntity>()
                                      .FirstOrDefaultAsync(x => x.InternalCode == internalCode);
                    return exists == null;
                }).WithMessage(ValidatorTranform.Exists("internalCode"));
        }
    }
}
