using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Students.Requests.Queries
{
    public class ListStudentRequest<T> : ListBaseRequest<T>
    {
        public bool IsGetMajor { get; set; }
    }
}
