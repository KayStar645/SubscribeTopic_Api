using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Faculties.Requests.Queries
{
    public class ListFacultyRequest<T> : ListBaseRequest<T>
    {
        public bool isGetDepartment { get; set; }
        public bool isGetDean { get; set; }
    }
}
