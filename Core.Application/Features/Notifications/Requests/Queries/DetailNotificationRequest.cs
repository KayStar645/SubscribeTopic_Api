using Core.Application.DTOs.Notification;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Notifications.Requests.Queries
{
    public class DetailNotificationRequest : DetailBaseRequest, IRequest<Result<NotificationDto>>
    {
        public bool isGetFaculty { get; set; }
    }
}
