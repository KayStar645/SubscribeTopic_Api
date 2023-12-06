using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.DTOs.Notification.Validators
{
    public class CreateNotificationDtoValidator : AbstractValidator<CreateNotificationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateNotificationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new NotificationDtoValidator(_unitOfWork));

            RuleFor(x => x.FacultyId)
                .MustAsync(async (id, token) =>
                {
                    var facultyEntityExists = await _unitOfWork.Repository<FacultyEntity>().GetByIdAsync(id);
                    return facultyEntityExists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("facultyId", "faculty"));
        }
    }
}
