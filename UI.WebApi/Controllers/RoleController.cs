using Core.Application.Exceptions;
using Core.Application.Interfaces.Identity;
using Core.Application.Models.Identity.Auths;
using Core.Application.Models.Identity.Roles;
using Core.Application.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Permission("Role.View")]
        public async Task<ActionResult<List<RoleResult>>> Get()
        {
            var response = await _roleService.GetList();

            return StatusCode(response.Code, response);
        }

        [HttpGet("Detail")]
        [Permission("Role.View")]
        public async Task<ActionResult<RoleResult>> Get(int pId)
        {
            var response = await _roleService.GetDetail(pId);

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        [Permission("Role.Create")]
        public async Task<ActionResult<RoleResult>> Create([FromBody] RoleRequest pRequest)
        {
            Result<RoleResult> response = await _roleService.CreateAsync(pRequest);

            return StatusCode(response.Code, response);
        }

        [HttpPut]
        [Permission("Role.Update")]
        public async Task<ActionResult<RoleResult>> Update([FromBody] RoleRequest pRequest)
        {
            var response = await _roleService.UpdateAsync(pRequest);

            return StatusCode(response.Code, response);
        }

        [HttpDelete]
        [Permission("Role.Delete")]
        public async Task<ActionResult<RoleResult>> Delete([FromQuery] int pId)
        {
            try
            {
                await _roleService.DeleteAsync(pId);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<RoleResult>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<RoleResult>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<RoleResult>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }

        [HttpPost("assign")]
        [Permission("Role.Assign")]
        public async Task<ActionResult<RoleResult>> AssignRoles([FromBody] AssignRoleRequest pRequest)
        {
            var response = await _roleService.AssignRoles(pRequest);

            return StatusCode(response.Code, response);
        }
    }
}
