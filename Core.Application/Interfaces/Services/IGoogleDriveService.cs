using Core.Application.Models.GoogleDrive;
using Core.Application.Responses;

namespace Core.Application.Interfaces.Services
{
    public interface IGoogleDriveService
    {
        Task<Result<UploadResponse>> UploadFilesToGoogleDrive(UploadRequest pRequest);
    }
}
