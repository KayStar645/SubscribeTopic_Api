using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;

namespace Core.Application.DTOs.Notification.Validators
{
    public class NotificationDtoValidator : AbstractValidator<INotificationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.FacultyId)
                .MustAsync(async (id, token) =>
                {
                    var facultyEntityExists = await _unitOfWork.Repository<FacultyEntity>().GetByIdAsync(id);
                    return facultyEntityExists != null || id == null;
                })
                .WithMessage(id => ValidatorTranform.NotExistsValueInTable("dacultyId", "faculty"));

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ValidatorTranform.Required("name"))
                .MaximumLength(190).WithMessage(ValidatorTranform.MaximumLength("name", 190));

            RuleFor(x => x.Describe)
                .MaximumLength(5000).WithMessage(ValidatorTranform.MaximumLength("describe", 5000));

            RuleFor(x => x.Image)
                .Must(image => string.IsNullOrEmpty(image) || Uri.TryCreate(image, UriKind.Absolute, out _))
                .WithMessage(ValidatorTranform.MustUrl("image"));

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
                .WithMessage(ValidatorTranform.MustUrls("images"));

        }
    }
}
