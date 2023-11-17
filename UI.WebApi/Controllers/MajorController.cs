using Core.Application.DTOs.Major;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Majors.Requests.Commands;
using Core.Application.Features.Majors.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/major")]
    [ApiController]
    public class MajorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MajorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách chuyên ngành theo ngành/khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// facultyId/industryId: required
        /// 
        /// </remarks>
        [HttpGet]
        [Permission("Major.View")]
        public async Task<ActionResult<List<MajorDto>>> Get([FromQuery] ListMajorRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin chuyên ngành theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Permission("Major.View")]
        public async Task<ActionResult<MajorDto>> Get([FromQuery] DetailMajorRequest request)
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
        [Permission("Major.Create")]
        public async Task<ActionResult<MajorDto>> Post([FromBody] CreateMajorDto majorRequest)
        {
            var command = new CreateMajorRequest { createMajorDto = majorRequest };
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
        [Permission("Major.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateMajorDto majorRequest)
        {
            var command = new UpdateMajorRequest { updateMajorDto = majorRequest };
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
        [Permission("Major.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Major> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<MajorDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<MajorDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<MajorDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
