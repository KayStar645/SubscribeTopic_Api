using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Notification;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculty;

namespace Core.Application.Features.Notifications.Requests.Queries
{
    public class ListNotificationRequest : ListBaseRequest<NotificationDto>
    {
        public bool? isGetFaculty { get; set; }

        public int? facultyId { get; set; }
    }

    public class NotificationDtoValidator : AbstractValidator<ListNotificationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<NotificationDto>());

            RuleFor(x => x.facultyId)
                .MustAsync(async (facultyId, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                        .FirstOrDefaultAsync(x => x.Id == facultyId);
                    return exists != null || facultyId == null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("facultyId"));
        }
    }
}
