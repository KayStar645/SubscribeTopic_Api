using Core.Application.DTOs.Group;
using Core.Application.Features.Groups.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách nhóm theo khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - facultyId: required
        /// </remarks>
        [HttpGet]
        [Permission("Group.View")]
        public async Task<ActionResult<List<GroupDto>>> Get([FromQuery] ListGroupRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin bộ môn theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Permission("Group.View")]
        public async Task<ActionResult> Get([FromQuery] DetailGroupRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }
    }
}
