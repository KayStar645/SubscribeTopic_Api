using Core.Application.DTOs.Industry;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Industries.Requests.Commands;
using Core.Application.Features.Industries.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class IndustryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IndustryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách ngành theo khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// FacultyId: request
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<List<IndustryDto>>> Get([FromQuery] ListIndustryRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin ngành theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        public async Task<ActionResult<IndustryDto>> Get([FromQuery] DetailIndustryRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm chuyên ngành
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Name: string, required, max(190)
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<IndustryDto>> Post([FromBody] CreateIndustryDto IndustryRequest)
        {
            var command = new CreateIndustryRequest { createIndustryDto = IndustryRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa chuyên ngành
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// </remarks>
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateIndustryDto IndustryRequest)
        {
            var command = new UpdateIndustryRequest { updateIndustryDto = IndustryRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa chuyên ngành
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Industry> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                var responses = Result<IndustryDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                switch (ex)
                {
                    case NotFoundException:
                        responses = Result<IndustryDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                        break;
                    case BadRequestException:
                        responses = Result<IndustryDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                        break;
                }
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
