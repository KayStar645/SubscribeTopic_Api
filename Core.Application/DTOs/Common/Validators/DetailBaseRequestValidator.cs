using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Common.Validators
{
    public class DetailBaseRequestValidator : AbstractValidator<DetailBaseRequest>
    {
        public DetailBaseRequestValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));
        }
    }
}
