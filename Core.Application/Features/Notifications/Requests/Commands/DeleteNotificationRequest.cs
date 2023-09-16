using MediatR;

namespace Core.Application.Features.Notifications.Requests.Commands
{
    public class DeleteNotificationRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
