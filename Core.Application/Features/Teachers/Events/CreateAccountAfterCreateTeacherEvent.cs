using Core.Application.Contracts.Persistence;
using Core.Application.Interfaces.Repositories;
using Core.Domain.Entities;
using Core.Domain.Entities.Identity;
using MediatR;

namespace Core.Application.Features.Teachers.Events
{
    public class CreateAccountAfterCreateTeacherEvent : INotification
    {
        public Teacher _teacher { get; set; }
        public IUserRepository _userRepository { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public CreateAccountAfterCreateTeacherEvent(Teacher teacher, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _teacher = teacher;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
    }

    public class CreateAccountAfterCreateTeacherHandler : INotificationHandler<CreateAccountAfterCreateTeacherEvent>
    {
        public async Task Handle(CreateAccountAfterCreateTeacherEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                User user = new User()
                {
                    UserName = pEvent._teacher.InternalCode,
                    Password = pEvent._teacher.InternalCode,
                };

                await pEvent._userRepository.CreateAsync(user);

                var userFind = await pEvent._userRepository.FindByNameAsync(user.UserName);
                var teacher = pEvent._teacher;
                teacher.UserId = userFind.Id;

                await pEvent._unitOfWork.Repository<Teacher>().UpdateAsync(teacher);
                await pEvent._unitOfWork.Save(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
