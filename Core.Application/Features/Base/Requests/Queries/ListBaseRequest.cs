using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Base.Requests.Queries
{
    public class ListBaseRequest<T> : IRequest<PaginatedResult<List<T>>>
    {
        public string? Filters { get; set; }
        public string? Sorts { get; set; }
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}
