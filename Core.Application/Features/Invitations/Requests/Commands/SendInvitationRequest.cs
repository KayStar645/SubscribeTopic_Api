using Core.Application.DTOs.Invitation;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Invitations.Requests.Commands
{
    public class SendInvitationRequest : IRequest<Result<InvitationDto>>
    {
        public SendInvitationDto? sendInvitationDto { get; set; }
    }
}
