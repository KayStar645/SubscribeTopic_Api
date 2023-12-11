using Core.Application.DTOs.ReportSchedule;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.ReportSchedule.Requests.Commands;
using Core.Application.Features.ReportSchedule.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/reportSchedule")]
    [ApiController]
    public class ReportScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách lịch theo vai trò của người đăng nhập (GV/SV)
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// </remarks>
        [HttpGet]
        [Permission("ReportSchedule.View")]
        public async Task<ActionResult> Get([FromQuery] ListReportScheduleRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Chi tiết một 1 lịch
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Permission("ReportSchedule.View")]
        public async Task<ActionResult> Get([FromQuery] DetailReportScheduleRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm lịch
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - ThesisId: Lịch của đề tài nào
        /// - DateTime: lớn hơn hiện tại
        /// - Location: 500
        /// - Type: W/R - Lịch hằng tuần/Lịch phản biện
        /// </remarks>
        [HttpPost]
        [Permission("ReportSchedule.Create")]
        public async Task<ActionResult> Post([FromBody] CreateReportScheduleDto request)
        {
            var command = new CreateReportScheduleRequest { createReportScheduleDto = request };
            var response = await _mediator.Send(command);

            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            var json = JsonConvert.SerializeObject(response, settings);

            return StatusCode(response.Code, json);
        }

        /// <summary>
        /// Sửa lịch
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - DateTime: lớn hơn hiện tại
        /// - Location: 500
        /// - Type: W/R - Lịch hằng tuần/Lịch phản biện
        /// </remarks>
        [HttpPut]
        [Permission("ReportSchedule.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateReportScheduleDto request)
        {
            var command = new UpdateReportScheduleRequest { updateReportScheduleDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa lịch
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Permission("ReportSchedule.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<ReportSchedule> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<ReportScheduleDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<ReportScheduleDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<ReportScheduleDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
