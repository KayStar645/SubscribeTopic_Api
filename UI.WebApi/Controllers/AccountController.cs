using Core.Application.Contracts.Identity;
using Core.Application.Models.Identity.Auths;
using Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authenticationService;
        public AccountController(IAuthService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Lấy danh sách user
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// </remarks>
        [HttpGet]
        [Permission("Account.View")]
        public async Task<ActionResult> Get()
        {
            var response = await _authenticationService.GetList();

            return StatusCode(response.Code, response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Result<AuthResponse>>> Login(AuthRequest request)
        {
            Result<AuthResponse> response = await _authenticationService.Login(request);

            return StatusCode(response.Code, response);
        }

        [HttpPost("register")]
        [Permission("Account.Create")]
        public async Task<ActionResult<Result<RegistrationResponse>>> Register(RegistrationRequest request)
        {
            Result<RegistrationResponse> response = await _authenticationService.Register(request);

            return StatusCode(response.Code, response);
        }
    }
}
