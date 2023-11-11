using Core.Application.DTOs.Feedback;
using Core.Application.DTOs.Thesis;
using Core.Application.Features.Feedbacks.Requests.Commands;
using Core.Application.Features.Thesiss.Requests.Commands;
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
        /// Thêm góp ý đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Content: string, required, max(6000)
        /// - thesisId: mã đề tài hợp lệ
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<FeedbackDto>> Post([FromBody] CreateFeedbackDto request)
        {
            var command = new CreateFeedbackRequest { createFeedbackDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }
    }
}
