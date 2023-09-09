using Core.Application.DTOs.Faculty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Faculties.Requests.Queries
{
    public class DetailFacultyRequest : DetailBaseRequest, IRequest<Result<FacultyDto>>
    {
        public bool IsGetDepartment { get; set; }
    }
}
