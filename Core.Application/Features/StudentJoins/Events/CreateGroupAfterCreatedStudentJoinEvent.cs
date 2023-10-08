using Core.Application.Contracts.Persistence;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.StudentJoins.Events
{
    public class CreateGroupAfterCreatedStudentJoinEvent : INotification
    {
        public StudentJoin studentJoin { get; set; }
        public IUnitOfWork unitOfWork { get; set; }

        public CreateGroupAfterCreatedStudentJoinEvent(StudentJoin studentJoin, IUnitOfWork unitOfWork)
        {
            this.studentJoin = studentJoin;
            this.unitOfWork = unitOfWork;
        }
    }

    public class CreateGroupAfterCreatedStudentJoinEventHandler : INotificationHandler<CreateGroupAfterCreatedStudentJoinEvent>
    {
        public async Task Handle(CreateGroupAfterCreatedStudentJoinEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                var group = new Group()
                {
                    Name = $"Group-{pEvent.studentJoin.Student.Name}",
                    CountMember = 1,
                    LeaderId = pEvent.studentJoin.Id
                };

                var newGroup = await pEvent.unitOfWork.Repository<Group>().AddAsync(group);
                await pEvent.unitOfWork.Save(cancellationToken);

                // Cập nhật lại mã nhóm cho sinh viên này
                var updateStudentJoin = pEvent.studentJoin;
                updateStudentJoin.GroupId = newGroup.Id;

                await pEvent.unitOfWork.Repository<StudentJoin>().UpdateAsync(updateStudentJoin);
                await pEvent.unitOfWork.Save(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
