using Core.Application.Contracts.Persistence;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.StudentJoins.Events
{
    public class UpdateGroupAfterDeletedStudentJoinEvent : INotification
    {
        public StudentJoin studentJoin { get; set; }
        public IUnitOfWork unitOfWork { get; set; }

        public UpdateGroupAfterDeletedStudentJoinEvent(StudentJoin studentJoin, IUnitOfWork unitOfWork)
        {
            this.studentJoin = studentJoin;
            this.unitOfWork = unitOfWork;
        }
    }

    public class UpdateGroupAfterDeletedStudentJoinEventHandler : INotificationHandler<UpdateGroupAfterDeletedStudentJoinEvent>
    {
        public async Task Handle(UpdateGroupAfterDeletedStudentJoinEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                var group = await pEvent.unitOfWork.Repository<Group>().GetByIdAsync(pEvent.studentJoin.GroupId);

                if (group.CountMember == 1)
                {
                    await pEvent.unitOfWork.Repository<Group>().DeleteAsync(group);
                    await pEvent.unitOfWork.Save(cancellationToken);
                }
                else
                {
                    group.CountMember -= 1;

                    if (group.LeaderId == pEvent.studentJoin.Id)
                    {
                        // Lấy thành viên khác làm trưởng nhóm
                        var student = await pEvent.unitOfWork.Repository<StudentJoin>()
                                                        .FirstOrDefaultAsync(x => x.GroupId == group.Id &&
                                                                                  x.Id != pEvent.studentJoin.StudentId);

                        group.LeaderId = student?.Id;
                    }
                    await pEvent.unitOfWork.Repository<Group>().UpdateAsync(group);
                    await pEvent.unitOfWork.Save(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
