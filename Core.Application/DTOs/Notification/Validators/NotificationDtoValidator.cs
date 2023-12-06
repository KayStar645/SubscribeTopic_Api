using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;

namespace Core.Application.DTOs.Notification.Validators
{
    public class NotificationDtoValidator : AbstractValidator<INotificationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTransform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTransform.MaximumLength("name", 190));

            RuleFor(x => x.Describe)
                .MaximumLength(5000).WithMessage(ValidatorTransform.MaximumLength("describe", 5000));

            RuleFor(x => x.Image)
                .Must(image => string.IsNullOrEmpty(image) || Uri.TryCreate(image, UriKind.Absolute, out _))
                .WithMessage(ValidatorTransform.MustUrl("image"));

            RuleFor(x => x.Images)
                .Must(images =>
                {
                    if (images == null)
                        return true;
                    foreach (var image in images)
                    {
                        if (!string.IsNullOrEmpty(image) && !Uri.TryCreate(image, UriKind.Absolute, out _))
                            return false;
                    }

                    return true;
                })
                .WithMessage(ValidatorTransform.MustUrls("images"));

        }
    }
}
