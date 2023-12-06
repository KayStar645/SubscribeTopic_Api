using Core.Application.Interfaces.Services;
using Core.Application.Models.GoogleDrive;
using Microsoft.AspNetCore.Mvc;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/googleDrive")]
    [ApiController]
    public class GoogleDriveController : ControllerBase
    {
        private readonly IGoogleDriveService _googleDriveService;

        public GoogleDriveController(IGoogleDriveService googleDriveService)
        {
            _googleDriveService = googleDriveService;
        }

        [HttpPost]
        //[Permission("GoogleDrive.Upload")]
        public async Task<ActionResult> UploadFile([FromQuery] UploadRequest pRequest)
        {
            var response = await _googleDriveService.UploadFilesToGoogleDrive(pRequest);

            return StatusCode(response.Code, response);
        }

        [HttpGet]
        //[Permission("GoogleDrive.GetFile")]
        public async Task<ActionResult> GetFileDrive([FromQuery] string pFilePath)
        {
            var response = await _googleDriveService.GetFileInfoFromGoogleDrive(pFilePath);

            return StatusCode(response.Code, response);
        }
    }
}
