using Core.Application.DTOs.Exchanges;
using Core.Application.Features.Exchanges.Requests.Commands;
using Core.Application.Features.Exchanges.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/exchange")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExchangeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy trao đổi theo công việc
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// jobId: required
        /// 
        /// </remarks>
        [HttpGet]
        [Permission("Exchange.View")]
        public async Task<ActionResult<List<ExchangeDto>>> Get([FromQuery] ListExchangeRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Trao đổi trong bài tập
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Content: string, required, max(6000)
        /// - jobId: mã đề tài hợp lệ
        /// </remarks>
        [HttpPost]
        [Permission("Exchange.Create")]
        public async Task<ActionResult<ExchangeDto>> Post([FromBody] CreateExchangeDto request)
        {
            var command = new CreateExchangeRequest { createExchangeDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }
    }
}
