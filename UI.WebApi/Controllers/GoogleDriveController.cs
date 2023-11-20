using Core.Application.Interfaces.Services;
using Core.Application.Responses;
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
        public async Task<ActionResult> UploadImage([FromForm] IFormFile file)
        {
            var result = await _googleDriveService.UploadImage(file);
            return Ok(result);
        }
    }
}
