using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Common.Validators
{
    public class DeleteBaseRequestValidator<T> : AbstractValidator<DeleteBaseRequest<T>>
    {
        public DeleteBaseRequestValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));
        }
    }
}
