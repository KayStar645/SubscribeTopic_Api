using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Notifications.Requests.Queries
{
    public class ListNotificationRequest<T> : ListBaseRequest<T>
    {
        public bool isGetFaculty { get; set; }
    }
}
