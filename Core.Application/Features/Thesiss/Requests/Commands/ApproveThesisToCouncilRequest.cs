using Core.Application.DTOs.Thesis;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Thesiss.Requests.Commands
{
    public class ApproveThesisToCouncilRequest : IRequest<Result<ThesisDto>>
    {
        public int? ThesisId { get; set; }
    }
}
