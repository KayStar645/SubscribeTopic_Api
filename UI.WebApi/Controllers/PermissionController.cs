using Core.Application.Interfaces.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/permission")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> Get()
        {
            var response = await _permissionService.GetList(Assembly.GetExecutingAssembly());

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        //[Permission("Permission.Create")]
        public async Task<ActionResult<List<string>>> Post()
        {
            var permission = await _permissionService.GetList(Assembly.GetExecutingAssembly());

            var response = await _permissionService.Create(permission.Data);

            return StatusCode(response.Code, response);
        }
    }
}
