using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Common.Validators
{
    public class ListBaseRequestValidator<T> : AbstractValidator<ListBaseRequest<T>>
    {
        public ListBaseRequestValidator()
        {
            RuleFor(x => x.page)
                .GreaterThanOrEqualTo(1)
                .WithMessage(ValidatorTransform.GreaterThanOrEqualTo("pageNumber", 1));

            RuleFor(x => x.pageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage(ValidatorTransform.GreaterThanOrEqualTo("pageSize", 1));
        }
    }
}
