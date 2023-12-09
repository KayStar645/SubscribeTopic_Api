using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Group;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using FluentValidation;
using MediatR;

namespace Core.Application.Features.Groups.Requests.Queries
{
    public class DetailGroupRequest : DetailBaseRequest, IRequest<Result<GroupDto>>
    {
        public bool? isGetLeader { get; set; }
        public bool? isGetGroupMeCurrent { get; set; }
        public bool? isGetMember { get; set; }
    }

    public class DetailGroupDtoValidator : AbstractValidator<DetailGroupRequest>
    {
        public DetailGroupDtoValidator(bool? isGetGroupMeCurrent)
        {
            if (isGetGroupMeCurrent != true)
            {
                RuleFor(x => x.id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));
            }
        }
    }
}
