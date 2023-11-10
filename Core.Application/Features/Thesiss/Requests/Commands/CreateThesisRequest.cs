using Core.Application.DTOs.Thesis;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Thesiss.Requests.Commands
{
    public class CreateThesisRequest : IRequest<Result<ThesisDto>>
    {
        public CreateThesisDto? createThesisDto { get; set; }
    }
}
