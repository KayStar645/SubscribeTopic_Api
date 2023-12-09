using Core.Application.DTOs.JobResults;
using Core.Application.Features.JobResults.Requests.Commands;
using Core.Application.Features.JobResults.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobResultsController : ControllerBase
    {

        private readonly IMediator _mediator;

        public JobResultsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách bài tập đã nộp theo công việc
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - jobId: required
        /// </remarks>
        [HttpGet]
        [Permission("JobResults.View")]
        public async Task<ActionResult> Get([FromQuery] ListJobResultsRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Nộp bài tập cho công việc dược giao
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Files
        /// </remarks>
        [HttpPost]
        [Permission("JobResults.Create")]
        public async Task<ActionResult> Post([FromBody] SubmitJobResultsDto request)
        {
            var command = new CreateJobResultsRequest { createJobResultsDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }
    }
}
