using Core.Application.DTOs.Faculty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Faculties.Requests.Queries
{
    public class ListFacultyRequest<T> : ListBaseRequest<T>
    {
        public bool isGetDepartment { get; set; }
        public bool isGetDean { get; set; }
    }
}
