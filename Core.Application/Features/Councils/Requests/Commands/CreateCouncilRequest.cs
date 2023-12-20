using Core.Application.DTOs.Council;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Councils.Requests.Commands
{
    public class CreateCouncilRequest : IRequest<Result<CouncilDto>>
    {
        public CreateCouncilDto? createCouncilDto { get; set; }
    }
}
