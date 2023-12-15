using Core.Application.DTOs.Duty;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Duties.Requests.Commands;
using Core.Application.Features.Duties.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/duty")]
    [ApiController]
    public class FacultyDutyController : ControllerBase
    {

        private readonly IMediator _mediator;

        public FacultyDutyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách nhiệm vụ của khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// 
        /// </remarks>
        [HttpGet]
        [Permission("Duty.View")]
        public async Task<ActionResult> Get([FromQuery] ListFacultyDutyRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin nhiệm vụ theo id
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Permission("Duty.View")]
        public async Task<ActionResult> Get([FromQuery] DetailFacultyDutyRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm nhiệm vụ
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Name: string, required, max(190)
        /// - Content: string, max(6000), ckeditor
        /// - Files:
        /// - TimeEnd: > hiện tại
        /// </remarks>
        [HttpPost]
        [Permission("Duty.Create")]
        public async Task<ActionResult> Post([FromBody] CreateFacultyDutyDto DutyRequest)
        {
            var command = new CreateFacultyDutyRequest { createFacultyDutyDto = DutyRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa nhiệm vụ
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// - Content: string, max(6000), ckeditor
        /// - Files:
        /// - TimeEnd: > hiện tại
        /// </remarks>
        [HttpPut]
        [Permission("Duty.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateDutyDto DutyRequest)
        {
            var command = new UpdateFacultyDutyRequest { updateDutyDto = DutyRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa nhiệm vụ
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Permission("Duty.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Duty> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<FacultyDutyDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<FacultyDutyDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<FacultyDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
