using FluentValidation;

namespace Core.Application.DTOs.Notification.Validators
{
    public class CreateNotificationDtoValidator : AbstractValidator<INotificationDto>
    {
        public CreateNotificationDtoValidator()
        {
            Include(new NotificationDtoValidator());
        }
    }
}
