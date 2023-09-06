using Shared.Interfaces;

namespace Shared
{
    public class PaginatedResult<T> : Result<T>
    {
        public PaginatedResult(T data)
        {
            Data = data;
        }

        public PaginatedResult(bool succeeded, T data = default, List<string> messages = null, int count = 0, int? pageNumber = 1, int? pageSize = 10, int code = 200)
        {
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            Data = data;
            CurrentPage = pageNumber;
            Succeeded = succeeded;
            Messages = messages;
            PageSize = pageSize;
            TotalPages = totalPages == 0 ? 1 : totalPages;
            TotalCount = count;
            Code = code;
        }

        public PaginatedResult(bool succeeded, List<string> messages = null, int code = 200)
        {
            CurrentPage = 1;
            Succeeded = succeeded;
            Messages = messages;
            PageSize = 10;
            TotalPages = 1;
            TotalCount = 0;
            Code = code;
        }

        public new T Data { get; set; } = default(T);
        public int? CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int? PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public static PaginatedResult<T> Create(T data, int count, int? pageNumber, int? pageSize, int code)
        {
            return new PaginatedResult<T>(true, data, null, count, pageNumber, pageSize, code);
        }
    }
}
