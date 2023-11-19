using Core.Application.DTOs.Feedback;
using Core.Application.Features.Feedbacks.Requests.Commands;
using Core.Application.Features.Feedbacks.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/feedback")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FeedbackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy góp ý theo đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// thesisId: required
        /// 
        /// </remarks>
        [HttpGet]
        [Permission("Feedback.View")]
        public async Task<ActionResult<List<FeedbackDto>>> Get([FromQuery] ListFeedbackRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm góp ý đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Content: string, required, max(6000)
        /// - thesisId: mã đề tài hợp lệ
        /// </remarks>
        [HttpPost]
        [Permission("Feedback.Create")]
        public async Task<ActionResult<FeedbackDto>> Post([FromBody] CreateFeedbackDto request)
        {
            var command = new CreateFeedbackRequest { createFeedbackDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }
    }
}
