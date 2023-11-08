using Core.Application.Contracts.Persistence;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.StudentJoins.Events
{
    public class AfterCreatedStudentJoinCreateGroupEvent : INotification
    {
        public StudentJoin _studentJoin { get; set; }
        public IUnitOfWork _unitOfWork { get; set; }

        public AfterCreatedStudentJoinCreateGroupEvent(StudentJoin studentJoin, IUnitOfWork unitOfWork)
        {
            _studentJoin = studentJoin;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterCreatedStudentJoinCreateGroupHandler : INotificationHandler<AfterCreatedStudentJoinCreateGroupEvent>
    {
        public async Task Handle(AfterCreatedStudentJoinCreateGroupEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                var group = new Group()
                {
                    Name = $"Group-{pEvent._studentJoin.Student.Name}",
                    CountMember = 1,
                    LeaderId = pEvent._studentJoin.Id
                };

                var newGroup = await pEvent._unitOfWork.Repository<Group>().AddAsync(group);
                await pEvent._unitOfWork.Save(cancellationToken);

                // Cập nhật lại mã nhóm cho sinh viên này
                var updateStudentJoin = pEvent._studentJoin;
                updateStudentJoin.GroupId = newGroup.Id;

                await pEvent._unitOfWork.Repository<StudentJoin>().UpdateAsync(updateStudentJoin);
                await pEvent._unitOfWork.Save(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
