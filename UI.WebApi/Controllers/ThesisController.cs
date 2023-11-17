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
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/Thesis")]
    [ApiController]
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
        [Permission("Thesis.View")]
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
        [Permission("Thesis.View")]
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
        /// - Name: string, required, max(190), unique
        /// - internalCode: string, required, max(190), unique
        /// </remarks>
        [HttpPost]
        [Permission("Thesis.Create")]
        public async Task<ActionResult<ThesisDto>> Post([FromBody] CreateThesisDto request)
        {
            var command = new CreateThesisRequest { createThesisDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190), unique
        /// - internalCode: string, required, max(190), unique
        /// </remarks>
        [HttpPut]
        [Permission("Thesis.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateThesisDto request)
        {
            var command = new UpdateThesisRequest { updateThesisDto = request };
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
        [Permission("Thesis.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Thesis> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }

        /// <summary>
        /// Sửa đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Status: Required
        /// D -> AR
        /// AR -> A/D
        /// 
        /// </remarks>
        [HttpPut("ChangeStatus")]
        [Permission("Thesis.Change")]
        public async Task<ActionResult> ChangeStatus([FromBody] ChangeStatusThesisDto request)
        {
            var command = new ChangeStatusThesisRequest { changeStatusThesis = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }
    }
}
