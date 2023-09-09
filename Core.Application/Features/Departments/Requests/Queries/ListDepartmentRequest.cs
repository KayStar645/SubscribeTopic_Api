using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Departments.Requests.Queries
{
    public class ListDepartmentRequest<T> : ListBaseRequest<T>
    {
        public bool IsGetFaculty { get; set; }
    }
}
