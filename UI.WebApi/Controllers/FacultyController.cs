using Core.Application.DTOs.Faculty;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Faculties.Requests.Commands;
using Core.Application.Features.Faculties.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/faculty")]
    [ApiController]
    [Authorize]
    public class FacultyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FacultyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Faculty.View")]
        public async Task<ActionResult<List<FacultyDto>>> Get([FromQuery] ListFacultyRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin khoa theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Authorize(Roles = "Faculty.View")]
        public async Task<ActionResult<FacultyDto>> Get([FromQuery] DetailFacultyRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Name: string, required, max(190)
        /// - PhoneNumber: string, length(10)
        /// - Email: string, email_format
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Faculty.Create")]
        public async Task<ActionResult<FacultyDto>> Post([FromBody] CreateFacultyDto FacultyRequest)
        {
            var command = new CreateFacultyRequest { createFacultyDto = FacultyRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// - PhoneNumber: string, length(10)
        /// - Email: string, email_format
        /// - Dean_TeacherId: Giảng viên của khoa
        /// </remarks>
        [HttpPut]
        [Authorize(Roles = "Faculty.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateFacultyDto FacultyRequest)
        {
            var command = new UpdateFacultyRequest { updateFacultyDto = FacultyRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Authorize(Roles = "Faculty.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Faculties> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<FacultyDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<FacultyDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<FacultyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
