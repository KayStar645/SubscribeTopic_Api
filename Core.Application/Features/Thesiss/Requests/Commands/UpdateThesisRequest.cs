using Core.Application.DTOs.Thesis;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Thesiss.Requests.Commands
{
    public class UpdateThesisRequest : IRequest<Result<ThesisDto>>
    {
        public UpdateThesisDto? updateThesisDto { get; set; }
    }
}
