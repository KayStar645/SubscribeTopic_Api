using Shared.Interfaces;

namespace Shared
{
    public class PaginatedResult<T> : Result<T>
    {
        public PaginatedResult(List<T> data)
        {
            Data = data;
        }

        public PaginatedResult(bool succeeded, List<T> data = default, List<string> messages = null, int count = 0, int pageNumber = 1, int pageSize = 10, int code = 200)
        {
            Data = data;
            CurrentPage = pageNumber;
            Succeeded = succeeded;
            Messages = messages;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Code = code;
        }

        public PaginatedResult(bool succeeded, List<string> messages = null, int code = 200)
        {
            Data = null;
            CurrentPage = 1;
            Succeeded = succeeded;
            Messages = messages;
            PageSize = 1;
            TotalPages = 1;
            TotalCount = 1;
            Code = code;
        }

        public new List<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public static PaginatedResult<T> Create(List<T> data, int count, int pageNumber, int pageSize)
        {
            return new PaginatedResult<T>(true, data, null, count, pageNumber, pageSize);
        }
    }
}
