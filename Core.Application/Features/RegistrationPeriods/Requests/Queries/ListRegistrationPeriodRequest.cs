using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.RegistrationPeriods.Requests.Queries
{
    public class ListRegistrationPeriodRequest<T> : ListBaseRequest<T>
    {
        public bool IsGetFaculty { get; set; }
    }
}
