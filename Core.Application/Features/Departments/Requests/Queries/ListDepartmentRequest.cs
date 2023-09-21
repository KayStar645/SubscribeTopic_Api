using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Departments.Requests.Queries
{
    public class ListDepartmentRequest<T> : ListBaseRequest<T>
    {
        public bool isGetFaculty { get; set; }
        public bool isGetHeadDepartment { get; set; }
    }
}
