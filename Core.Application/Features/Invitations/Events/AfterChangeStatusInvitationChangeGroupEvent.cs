using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Invitation;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Invitations.Events
{
    public class AfterChangeStatusInvitationChangeGroupEvent : INotification
    {
        public Group _group { get; set; }
        public InvitationDto _invitationDto { get; set; }
        public IUnitOfWork _unitOfWork { get; set; }
        public IMapper _mapper { get; set; }

        public AfterChangeStatusInvitationChangeGroupEvent(Group group, InvitationDto invitationDto,
                    IUnitOfWork unitOfWork, IMapper mapper)
        {
            _group = group;
            _invitationDto = invitationDto;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
    }

    public class AfterChangeStatusInvitationChangeGroupHandler : INotificationHandler<AfterChangeStatusInvitationChangeGroupEvent>
    {
        public async Task Handle(AfterChangeStatusInvitationChangeGroupEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            if (pEvent._invitationDto.Status == Invitation.STATUS_ACCEPT)
            {
                /* Kiểm tra nhóm cũ: */
                do
                {
                    var oldGroup = pEvent._group;
                    // 1 thành viên: Xóa Group cũ
                    if (oldGroup.CountMember == 1)
                    {
                        await pEvent._unitOfWork.Repository<Group>().DeleteAsync(pEvent._group);
                        await pEvent._unitOfWork.Save();
                        break;
                    }
                    // Nhiều thành viên
                    // Trưởng nhóm: Chuyển trưởng nhóm sang 1 thành viên bất kì, giảm số lượng thành viên
                    if(oldGroup.LeaderId == pEvent._invitationDto.StudentJoinId)
                    {
                        var newLeader = await pEvent._unitOfWork.Repository<StudentJoin>()
                                            .FindByCondition(x => x.GroupId == oldGroup.Id && x.Id != oldGroup.LeaderId)
                                            .FirstOrDefaultAsync();
                        oldGroup.LeaderId = newLeader.Id;
                    }

                    // Thành viên: giảm số lượng thành viên
                    oldGroup.CountMember -= 1;
                    await pEvent._unitOfWork.Repository<Group>().UpdateAsync(oldGroup);
                    await pEvent._unitOfWork.Save();
                }
                while (false);

                // Tăng số lượng thành viên trong Group mới
                var newGroup = await pEvent._unitOfWork.Repository<Group>()
                                        .FirstOrDefaultAsync(x => x.Id == pEvent._invitationDto.GroupId);
                newGroup.CountMember += 1;
                await pEvent._unitOfWork.Repository<Group>().UpdateAsync(newGroup);
                await pEvent._unitOfWork.Save();
            }
        }

    }
}
