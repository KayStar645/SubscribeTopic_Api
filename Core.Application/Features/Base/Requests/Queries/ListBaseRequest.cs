using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Base.Requests.Queries
{
    public class ListBaseRequest<T> : IRequest<PaginatedResult<List<T>>>
    {
        public string? filters { get; set; }
        public string? sorts { get; set; }
        public int? page { get; set; } = 1;
        public int? pageSize { get; set; } = 10;
        public bool isAllDetail { get; set; }
    }
}
