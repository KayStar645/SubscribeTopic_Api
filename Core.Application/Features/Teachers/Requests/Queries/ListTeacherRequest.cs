using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Teachers.Requests.Queries
{
    public class ListDepartmentRequest<T> : ListBaseRequest<T>
    {
        public bool IsGetDepartment { get; set; }
    }
}
