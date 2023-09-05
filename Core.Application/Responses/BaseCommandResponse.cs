namespace Core.Application.Responses
{
    public class BaseCommandResponse
    {
        public int Id { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }

    //public class BaseCommandResponse<T>
    //{
    //    public T? Data { get; set; }
    //    public object? AdditionalData { get; set; }
    //    public string? Message { get; set; }
    //    public int StatusCode { get; set; }
    //    public string Code { get; set; }

    //    public BaseCommandResponse(int statusCode, string code, T? data, object? additionalData = null)
    //    {
    //        this.Data = data;
    //        this.AdditionalData = additionalData;
    //        this.StatusCode = statusCode;
    //        this.Code = code;
    //    }

    //    public BaseCommandResponse(int statusCode, string code, string? message)
    //    {
    //        this.StatusCode = statusCode;
    //        this.Code = code;
    //        this.Message = message;
    //    }
    //}
}
