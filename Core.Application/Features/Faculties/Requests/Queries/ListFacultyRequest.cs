using Core.Application.DTOs.Faculty;
using Core.Application.Features.Base.Requests.Queries;

namespace Core.Application.Features.Faculties.Requests.Queries
{
    public class ListFacultyRequest : ListBaseRequest<FacultyDto>
    {
        public bool isGetDepartment { get; set; }
        public bool isGetDean { get; set; }
    }
}
