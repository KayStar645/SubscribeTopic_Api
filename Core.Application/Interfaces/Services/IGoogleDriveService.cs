using Core.Application.Models.GoogleDrive;
using Core.Application.Responses;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Interfaces.Services
{
    public interface IGoogleDriveService
    {
        Task<Result<UploadResponse>> UploadFilesToGoogleDrive(UploadRequest pRequest);
    }
}
