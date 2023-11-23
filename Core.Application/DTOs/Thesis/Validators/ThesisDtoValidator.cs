using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Thesis.Validators
{
    public class ThesisDtoValidator : AbstractValidator<IThesisDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ThesisDtoValidator(IUnitOfWork unitOfWork, int? minQuantity)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.MinQuantity)
                .NotEmpty().WithMessage(ValidatorTransform.Required("minQuantity"))
                .GreaterThanOrEqualTo(1).WithMessage(ValidatorTransform.GreaterThanOrEqualTo("minQuantity", 1))
                .LessThanOrEqualTo(5).WithMessage(ValidatorTransform.LessThanOrEqualTo("minQuantity", 5));

            RuleFor(x => x.MaxQuantity)
                .NotEmpty().WithMessage(ValidatorTransform.Required("maxQuantity"))
                .GreaterThanOrEqualTo((int)minQuantity).WithMessage(ValidatorTransform.GreaterThanOrEqualTo("maxQuantity", (int)minQuantity))
                .LessThanOrEqualTo(5).WithMessage(ValidatorTransform.LessThanOrEqualTo("maxQuantity", 5));
        }
    }
}
