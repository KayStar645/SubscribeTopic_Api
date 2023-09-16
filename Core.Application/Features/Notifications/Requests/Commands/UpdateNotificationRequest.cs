using Core.Application.DTOs.Notification;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Notifications.Requests.Commands
{
    public class UpdateNotificationRequest : IRequest<Result<NotificationDto>>
    {
        public UpdateNotificationDto? UpdateNotificationDto { get; set; }
    }
}
