using Core.Application.DTOs.Invitation;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Invitations.Request.Commands
{
    public class ChangeStatusRequest : IRequest<Result<InvitationDto>>
    {
        public ChangeStatusInvitationDto? changeStatusInvitationDto { get; set; }
    }

}
