using Core.Application.DTOs.Council;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Councils.Requests.Commands;
using Core.Application.Features.Councils.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouncilController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CouncilController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách hội đồng theo khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - facultyId: required
        /// </remarks>
        [HttpGet]
        [Permission("Council.View")]
        public async Task<ActionResult> Get([FromQuery] ListCouncilRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin 1 hội đồng
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Permission("Council.View")]
        public async Task<ActionResult> Get([FromQuery] DetailCouncilRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Hội đồng
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - InternalCode: string, required, max(50)
        /// - Name: string, required, max(190)
        /// - ProtectionDay: datetime > day
        /// - Location:
        /// - CommissionerDto: 
        ///     + TeacherId hợp lệ
        ///     + Position: M: Ủy viên (có thể nhiều người), C: Chủ tịch (1 người), S: Thư kí (1 người)
        /// </remarks>
        [HttpPost]
        [Permission("Council.Create")]
        public async Task<ActionResult> Post([FromBody] CreateCouncilDto request)
        {
            var command = new CreateCouncilRequest { createCouncilDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa hội đồng
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - InternalCode: string, required, max(50)
        /// - Name: string, required, max(190)
        /// - ProtectionDay: datetime > day
        /// - Location:
        /// - CommissionerDto: 
        ///     + TeacherId hợp lệ
        ///     + Position: M: Ủy viên (có thể nhiều người), C: Chủ tịch (1 người), S: Thư kí (1 người)
        /// </remarks>
        [HttpPut]
        [Permission("Council.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateCouncilDto request)
        {
            var command = new UpdateCouncilRequest { updateCouncilDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa hội đồng
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Permission("Council.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Council> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<CouncilDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<CouncilDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<CouncilDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
