using Core.Application.DTOs.Council;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Councils.Requests.Commands
{
    public class SelectThesisForCouncilRequest : IRequest<Result<CouncilDto>>
    {
        public SelectThesisForCouncilDto? selectThesisForCouncilDto { get; set; }
    }
}
