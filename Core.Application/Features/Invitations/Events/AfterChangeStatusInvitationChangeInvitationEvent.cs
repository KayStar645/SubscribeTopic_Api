using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Invitation;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Invitations.Events
{
    public class AfterChangeStatusInvitationChangeInvitationEvent : INotification
    {
        public InvitationDto _invitationDto { get; set; }
        public IUnitOfWork _unitOfWork { get; set; }
        public IMapper _mapper { get; set; }

        public AfterChangeStatusInvitationChangeInvitationEvent(InvitationDto invitationDto, 
                    IUnitOfWork unitOfWork, IMapper mapper)
        {
            _invitationDto = invitationDto;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
    }

    public class AfterChangeStatusInvitationChangeInvitationHandler : INotificationHandler<AfterChangeStatusInvitationChangeInvitationEvent>
    {
        public async Task Handle(AfterChangeStatusInvitationChangeInvitationEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            if(pEvent._invitationDto.Status == Invitation.STATUS_CANCEL)
            {
                var invitation = pEvent._mapper.Map<Invitation>(pEvent._invitationDto);

                await pEvent._unitOfWork.Repository<Invitation>().DeleteAsync(invitation);

                await pEvent._unitOfWork.Save();
            }    
        }

    }
}
