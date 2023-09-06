using Core.Application.DTOs.Teacher;
using MediatR;
using Shared;

namespace Core.Application.Features.Base.Requests.Queries
{
    public class GetListRequest : IRequest<PaginatedResult<List<ListTeacherDto>>>
    {
        public string? Filters { get; set; }
        public string? Sorts { get; set; }
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
