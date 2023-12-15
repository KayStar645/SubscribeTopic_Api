using Core.Application.DTOs.Point;
using Core.Application.Features.Points.Requests.Commands;
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
        /// Chấm điểm cho sv
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - StudentJoinId: 
        /// - Score: 0 -> 10
        /// </remarks>
        [HttpPost]
        [Permission("Point.Create")]
        public async Task<ActionResult> Post([FromBody] CreateOrUpdatePointDto request)
        {
            try
            {
                var command = new CreateOrUpdatePointRequest { createOrUpdatePointDto = request };
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
