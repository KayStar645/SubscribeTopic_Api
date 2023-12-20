using Core.Application.DTOs.Council;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Councils.Requests.Queries
{
    public class DetailCouncilRequest : DetailBaseRequest, IRequest<Result<CouncilDto>>
    {

    }
}
