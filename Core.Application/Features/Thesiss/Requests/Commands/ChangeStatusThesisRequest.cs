using Core.Application.DTOs.Thesis;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Thesiss.Requests.Commands
{
    public class ChangeStatusThesisRequest : IRequest<Result<ThesisDto>>
    {
        public ChangeStatusThesisDto? changeStatusThesis { get; set; }
    }
}
