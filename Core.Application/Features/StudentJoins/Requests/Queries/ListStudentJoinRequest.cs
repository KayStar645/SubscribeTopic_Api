using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.StudentJoins.Requests.Queries
{
    public class ListStudentJoinRequest<T> : ListBaseRequest<T>
    {
        public bool isGetStudent { get; set; }
        public bool isGetRegistrationPeriod { get; set; }
    }
}
