using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Thesiss.Requests.Commands;
using Core.Application.Features.Thesiss.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/Thesis")]
    [ApiController]
    //[Authorize]
    public class ThesisController : Controller
    {
        private readonly IMediator _mediator;

        public ThesisController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách đề tài theo ngành/khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// facultyId/industryId: required
        /// 
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<List<ThesisDto>>> Get([FromQuery] ListThesisRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin đề tài theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        public async Task<ActionResult<ThesisDto>> Get([FromQuery] DetailThesisRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Name: string, required, max(190)
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<ThesisDto>> Post([FromBody] CreateThesisDto ThesisRequest)
        {
            var command = new CreateThesisRequest { createThesisDto = ThesisRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// </remarks>
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateThesisDto ThesisRequest)
        {
            var command = new UpdateThesisRequest { updateThesisDto = ThesisRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Thesis> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                var responses = Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                switch (ex)
                {
                    case NotFoundException:
                        responses = Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                        break;
                    case BadRequestException:
                        responses = Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                        break;
                }
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
