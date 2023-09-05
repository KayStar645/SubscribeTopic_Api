using Core.Application.Features.Teachers.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Teacher.Validators
{
    public class GetTeacherListRequestValidator : AbstractValidator<GetTeacherListRequest>
    {
        public GetTeacherListRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage(ValidatorTranform.GreaterThanOrEqualTo("PageNumber", 1));

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage(ValidatorTranform.GreaterThanOrEqualTo("PageSize", 1));
        }
    }
}
