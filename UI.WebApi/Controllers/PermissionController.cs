using Core.Application.Interfaces.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        //[Authorize(Roles = "Permission.View")]
        public async Task<ActionResult<List<string>>> Get()
        {
            var response = await _permissionService.GetList(Assembly.GetExecutingAssembly());

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        //[Authorize(Roles = "Permission.Create")]
        public async Task<ActionResult<List<string>>> Post()
        {
            var permission = await _permissionService.GetList(Assembly.GetExecutingAssembly());

            var response = await _permissionService.Create(permission.Data);

            return StatusCode(response.Code, response);
        }
    }
}
