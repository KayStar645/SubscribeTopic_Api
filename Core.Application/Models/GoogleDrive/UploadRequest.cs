using Microsoft.AspNetCore.Http;

namespace Core.Application.Models.GoogleDrive
{
    public class UploadRequest
    {
        public IFormFile? File { get; set; }

        public string? FileName { get; set; }
    }
}
