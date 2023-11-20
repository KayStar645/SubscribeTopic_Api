using Core.Application.Responses;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Interfaces.Services
{
    public interface IGoogleDriveService
    {
        Task<Result<string>> UploadImage(IFormFile file);
    }
}
