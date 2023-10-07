using Core.Application.DTOs.Group;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Groups.Requests.Queries
{
    public class DetailGroupRequest : DetailBaseRequest, IRequest<Result<GroupDto>>
    {
        public bool? isGetLeader { get; set; }
        public bool? isGetMember { get; set; }
    }
}
