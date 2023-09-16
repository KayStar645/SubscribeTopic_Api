using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.Notification.Validators
{
    public class UpdateNotificationDtoValidator : AbstractValidator<INotificationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateNotificationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new NotificationDtoValidator(_unitOfWork));
        }
    }
}
