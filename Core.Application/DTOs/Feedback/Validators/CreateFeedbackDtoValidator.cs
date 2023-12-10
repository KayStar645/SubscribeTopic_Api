using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;

namespace Core.Application.DTOs.Feedback.Validator
{
    internal class CreateFeedbackDtoValidator : AbstractValidator<CreateFeedbackDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateFeedbackDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(ValidatorTransform.Required("content"))
                .MaximumLength(6000).WithMessage(ValidatorTransform.MaximumLength("content", 6000));

            RuleFor(x => x.ThesisId)
               .MustAsync(async (id, token) =>
               {
                   var exists = await _unitOfWork.Repository<ThesisEntity>().GetByIdAsync(id);
                   return exists != null;
               })
               .WithMessage(id => ValidatorTransform.NotExistsValueInTable("thesisId", "thesiss"));
        }
    }
}
