using Core.Application.Contracts.Identity;
using Core.Application.Models.Identity.Auths;
using Core.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("login")]
        public async Task<ActionResult<Result<AuthResponse>>> Login(AuthRequest request)
        {
            Result<AuthResponse> response = await _authenticationService.Login(request);

            return StatusCode(response.Code, response);
        }

        [HttpPost("register")]
        [Authorize(Roles = "Account.Create")]
        public async Task<ActionResult<Result<RegistrationResponse>>> Register(RegistrationRequest request)
        {
            Result<RegistrationResponse> response = await _authenticationService.Register(request);

            return StatusCode(response.Code, response);
        }
    }
}
