using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.RegistrationPeriods.Requests.Commands;
using Core.Application.Features.RegistrationPeriods.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/registrationPeriod")]
    [ApiController]
    //[Authorize]
    public class RegistrationPeriodController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegistrationPeriodController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách đợt đăng ký theo khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<List<RegistrationPeriodDto>>> Get([FromQuery] ListRegistrationPeriodRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin đợt đăng ký theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        public async Task<ActionResult<RegistrationPeriodDto>> Get([FromQuery] DetailRegistrationPeriodRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm đợt đăng ký
        /// </summary>
        /// <remarks>
        /// Ràng buộc:
        /// - Semester: string, in ["Học kỳ 1", "Học kỳ 2", "Học kỳ 3"]
        /// - TimeStart: DateTime, In or after Today (TimeStart >= Now)
        /// - TimeEnd: DateTime, After TimeStart (TimeEnd > TimeStart)
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<RegistrationPeriodDto>> Post([FromBody] CreateRegistrationPeriodDto periodRequest)
        {
            var command = new CreateRegistrationPeriodRequest { CreateRegistrationPeriodDto = periodRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa đợt đăng ký
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Semester: string, in ["Học kỳ 1", "Học kỳ 2", "Học kỳ 3"]
        /// - TimeStart: DateTime, In or after Today (TimeStart >= Now)
        /// - TimeEnd: DateTime, After TimeStart (TimeEnd > TimeStart)
        /// </remarks>
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateRegistrationPeriodDto periodRequest)
        {
            var command = new UpdateRegistrationPeriodRequest { UpdateRegistrationPeriodDto = periodRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa đợt đăng ký
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<RegistrationPeriod> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<RegistrationPeriodDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<RegistrationPeriodDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<RegistrationPeriodDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}