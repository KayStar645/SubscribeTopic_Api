using Core.Application.DTOs.Feedback;
using Core.Application.DTOs.Thesis;
using Core.Application.Features.Feedbacks.Requests.Commands;
using Core.Application.Features.Feedbacks.Requests.Queries;
using Core.Application.Features.Thesiss.Requests.Commands;
using Core.Application.Features.Thesiss.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [Authorize(Roles = "Feedback.View")]
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
        [Authorize(Roles = "Feedback.Create")]
        public async Task<ActionResult<FeedbackDto>> Post([FromBody] CreateFeedbackDto request)
        {
            var command = new CreateFeedbackRequest { createFeedbackDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }
    }
}
