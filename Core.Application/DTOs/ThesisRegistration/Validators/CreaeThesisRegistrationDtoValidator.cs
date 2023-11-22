using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;

namespace Core.Application.DTOs.ThesisRegistration.Validators
{
    public class CreaeThesisRegistrationDtoValidator : AbstractValidator<CreateThesisRegistrationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreaeThesisRegistrationDtoValidator(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.ThesisId)
                .MustAsync(async (thesisId, token) =>
                {
                    var exists = await _unitOfWork.Repository<ThesisEntity>().GetByIdAsync(thesisId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("thesisId", "thesis"));
        }
    }
}
