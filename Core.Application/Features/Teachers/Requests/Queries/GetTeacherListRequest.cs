using Core.Application.DTOs.Teacher;
using MediatR;
using Shared;

namespace Core.Application.Features.Teachers.Requests.Queries
{
    public class GetTeacherListRequest : IRequest<PaginatedResult<TeacherListDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
