using Core.Application.DTOs.Notification;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Notifications.Requests.Commands
{
    public class CreateNotificationRequest : IRequest<Result<NotificationDto>>
    {
        public CreateNotificationDto? createNotificationDto { get; set; }
    }
}
