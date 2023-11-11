using Core.Application.Contracts.Persistence;
using Core.Application.Interfaces.Repositories;
using Core.Domain.Entities;
using Core.Domain.Entities.Identity;
using MediatR;

namespace Core.Application.Features.Students.Events
{
    public class AfterCreateStudentCreateAccountEvent : INotification
    {
        public Student _student { get; set; }
        public IUserRepository _userRepository { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterCreateStudentCreateAccountEvent(Student student, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _student = student;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterCreateStudentCreateAccountHandler : INotificationHandler<AfterCreateStudentCreateAccountEvent>
    {
        public async Task Handle(AfterCreateStudentCreateAccountEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                User user = new User()
                {
                    UserName = pEvent._student.InternalCode,
                    Password = pEvent._student.InternalCode,
                };

                await pEvent._userRepository.CreateAsync(user);

                var userFind = await pEvent._userRepository.FindByNameAsync(user.UserName);
                var student = pEvent._student;
                student.UserId = userFind.Id;

                await pEvent._unitOfWork.Repository<Student>().UpdateAsync(student);
                await pEvent._unitOfWork.Save(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
