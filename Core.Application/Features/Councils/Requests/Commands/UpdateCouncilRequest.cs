using Core.Application.DTOs.Council;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Councils.Requests.Commands
{
    public class UpdateCouncilRequest : IRequest<Result<CouncilDto>>
    {
        public UpdateCouncilDto? updateCouncilDto { get; set; }
    }
}
