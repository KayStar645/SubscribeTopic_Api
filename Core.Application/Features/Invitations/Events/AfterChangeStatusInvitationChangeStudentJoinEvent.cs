using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Invitation;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Invitations.Events
{
    public class AfterChangeStatusInvitationChangeStudentJoinEvent : INotification
    {
        public InvitationDto _invitationDto { get; set; }
        public IUnitOfWork _unitOfWork { get; set; }
        public IMapper _mapper { get; set; }

        public AfterChangeStatusInvitationChangeStudentJoinEvent(InvitationDto invitationDto,
                    IUnitOfWork unitOfWork, IMapper mapper)
        {
            _invitationDto = invitationDto;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
    }

    public class AfterChangeStatusInvitationChangeStudentJoinHandler : INotificationHandler<AfterChangeStatusInvitationChangeStudentJoinEvent>
    {
        public async Task Handle(AfterChangeStatusInvitationChangeStudentJoinEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            if (pEvent._invitationDto.Status == Invitation.STATUS_ACCEPT)
            {
                // Cập nhật lại Group mới cho sinh viên
                var studentJoin = await pEvent._unitOfWork.Repository<StudentJoin>()
                                    .FindByCondition(x => x.Id == pEvent._invitationDto.StudentJoinId)
                                    .FirstOrDefaultAsync();
                studentJoin.GroupId = pEvent._invitationDto.GroupId;
                await pEvent._unitOfWork.Repository<StudentJoin>().UpdateAsync(studentJoin);
                await pEvent._unitOfWork.Save();
            }    
        }

    }
}
