using Core.Application.Contracts.Persistence;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.StudentJoins.Events
{
    public class UpdateGroupAfterDeletedStudentJoinEvent : INotification
    {
        public StudentJoin _studentJoin { get; set; }
        public IUnitOfWork _unitOfWork { get; set; }

        public UpdateGroupAfterDeletedStudentJoinEvent(StudentJoin studentJoin, IUnitOfWork unitOfWork)
        {
            _studentJoin = studentJoin;
            _unitOfWork = unitOfWork;
        }
    }

    public class UpdateGroupAfterDeletedStudentJoinEventHandler : INotificationHandler<UpdateGroupAfterDeletedStudentJoinEvent>
    {
        public async Task Handle(UpdateGroupAfterDeletedStudentJoinEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                var group = await pEvent._unitOfWork.Repository<Group>().GetByIdAsync(pEvent._studentJoin.GroupId);

                if (group.CountMember == 1)
                {
                    await pEvent._unitOfWork.Repository<Group>().DeleteAsync(group);
                    await pEvent._unitOfWork.Save(cancellationToken);
                }
                else
                {
                    group.CountMember -= 1;

                    if (group.LeaderId == pEvent._studentJoin.Id)
                    {
                        // Lấy thành viên khác làm trưởng nhóm
                        var student = await pEvent._unitOfWork.Repository<StudentJoin>()
                                                        .FirstOrDefaultAsync(x => x.GroupId == group.Id &&
                                                                                  x.Id != pEvent._studentJoin.StudentId);

                        group.LeaderId = student?.Id;
                    }
                    await pEvent._unitOfWork.Repository<Group>().UpdateAsync(group);
                    await pEvent._unitOfWork.Save(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
