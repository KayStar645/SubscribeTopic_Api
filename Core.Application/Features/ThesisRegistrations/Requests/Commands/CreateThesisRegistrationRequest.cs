using Core.Application.DTOs.ThesisRegistration;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.ThesisRegistrations.Requests.Commands
{
    public class CreateThesisRegistrationRequest : IRequest<Result<ThesisRegistrationDto>>
    {
        public CreateThesisRegistrationDto? createThesisRegistrationDto { get; set; }
    }
}
