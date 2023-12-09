using Core.Application.DTOs.Invitation;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Invitations.Requests.Commands
{
    public class ChangeStatusInvitationRequest : IRequest<Result<InvitationDto>>
    {
        public ChangeStatusInvitationDto? changeStatusInvitation { get; set; }
    }

}
