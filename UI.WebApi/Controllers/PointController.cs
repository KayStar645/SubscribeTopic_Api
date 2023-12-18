using Core.Application.DTOs.Point;
using Core.Application.Features.Points.Requests.Commands;
using Core.Application.Features.Points.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase
    {

        private readonly IMediator _mediator;

        public PointController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách điểm theo khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// thesisId: required
        /// 
        /// </remarks>
        [HttpGet("Faculty")]
        [Permission("Point.Faculty.View")]
        public async Task<ActionResult> Get([FromQuery] ListPointOfFacultyRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy danh sách điểm theo nhóm
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// thesisId: required
        /// 
        /// </remarks>
        [HttpGet("Group")]
        [Permission("Point.Group.View")]
        public async Task<ActionResult> Get([FromQuery] ListPointOfGroupRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy danh sách điểm theo đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// thesisId: required
        /// 
        /// </remarks>
        [HttpGet("Thesis")]
        [Permission("Point.Thesis.View")]
        public async Task<ActionResult> Get([FromQuery] ListPointOfThesisRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Chấm điểm cho sv
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - StudentJoinId: 
        /// - Score: 0 -> 10
        /// </remarks>
        [HttpPost]
        [Permission("Point.Create")]
        public async Task<ActionResult> Post([FromBody] UpdatePointDto request)
        {
            try
            {
                var command = new UpdatePointRequest { updatePointDto = request };
                var response = await _mediator.Send(command);

                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
