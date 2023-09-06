using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Common.Validators
{
    public class GetListRequestValidator : AbstractValidator<GetListRequest>
    {
        public GetListRequestValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage(ValidatorTranform.GreaterThanOrEqualTo("PageNumber", 1));

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage(ValidatorTranform.GreaterThanOrEqualTo("PageSize", 1));
        }
    }
}
