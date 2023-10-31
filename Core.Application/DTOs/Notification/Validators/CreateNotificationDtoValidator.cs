using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Notification.Validators
{
    public class CreateNotificationDtoValidator : AbstractValidator<INotificationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateNotificationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new NotificationDtoValidator(_unitOfWork));
        }
    }
}
