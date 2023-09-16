using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Notification.Validators
{
    public class NotificationDtoValidator : AbstractValidator<INotificationDto>
    {
        public NotificationDtoValidator()
        {
            RuleFor(x => x.InternalCode)
                 .NotEmpty().WithMessage(ValidatorTranform.Required("internalCode"))
                .MaximumLength(50).WithMessage(ValidatorTranform.MaximumLength("internalCode", 50));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190));

            // Validator các trường hợp khác nữa
        }
    }
}
