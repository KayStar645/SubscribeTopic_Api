using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using IndustryEntity = Core.Domain.Entities.Industry;

namespace Core.Application.DTOs.Major.Validators
{
    public class MajorDtoValidator : AbstractValidator<IMajorDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MajorDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.IndustryId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<IndustryEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("industryId", "industries"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190));
        }
    }
}
