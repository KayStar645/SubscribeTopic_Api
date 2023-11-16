using Core.Application.DTOs.Industry;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Industries.Requests.Commands;
using Core.Application.Features.Industries.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Industry.View")]
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
        [Authorize(Roles = "Industry.View")]
        public async Task<ActionResult<IndustryDto>> Get([FromQuery] DetailIndustryRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm ngành
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Name: string, required, max(190)
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Industry.Create")]
        public async Task<ActionResult<IndustryDto>> Post([FromBody] CreateIndustryDto IndustryRequest)
        {
            var command = new CreateIndustryRequest { createIndustryDto = IndustryRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa ngành
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// </remarks>
        [HttpPut]
        [Authorize(Roles = "Industry.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateIndustryDto IndustryRequest)
        {
            var command = new UpdateIndustryRequest { updateIndustryDto = IndustryRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa ngành
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Authorize(Roles = "Industry.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Industry> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<IndustryDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<IndustryDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<IndustryDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
