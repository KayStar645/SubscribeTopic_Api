using Core.Application.DTOs.FacultyDuty;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.FacultyDuties.Requests.Commands;
using Core.Application.Features.FacultyDuties.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/facultyduty")]
    [ApiController]
    public class FacultyDutyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FacultyDutyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách nhiệm vụ của khoa giao cho bộ môn
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - facultyId/departmentId: required
        /// </remarks>
        [HttpGet]
        [Permission("FacultyDuty.View")]
        public async Task<ActionResult> Get([FromQuery] ListFacultyDutyRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin nhiệm vụ của khoa theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Permission("FacultyDuty.View")]
        public async Task<ActionResult> Get([FromQuery] DetailFacultyDutyRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm nhiệm vụ của khoa giao cho bộ môn
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Name: string, required, max(190)
        /// - NumberOfThesis: int, required, range(1, max)
        /// - TimeStart: DateTime, In or after Today (TimeStart >= Now)
        /// - TimeEnd: DateTime, required, After TimeStart (TimeEnd > TimeStart)
        /// - Image: string, must be url
        /// - File: string, must be a valid file name (Ex: filename.extension)
        /// </remarks>
        [HttpPost]
        [Permission("FacultyDuty.Create")]
        public async Task<ActionResult> Post([FromBody] CreateFacultyDutyDto request)
        {
            var command = new CreateFacultyDutyRequest { CreateFacultyDutyDto = request };
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
        /// Sửa nhiệm vụ của khoa giao bộ môn
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// - NumberOfThesis: int, required, range(1, max)
        /// - TimeStart: DateTime, In or after Today (TimeStart >= Now)
        /// - TimeEnd: DateTime, required, After TimeStart (TimeEnd > TimeStart)
        /// - Image: string, must be url
        /// - File: string, must be a valid file name (Ex: filename.extension)
        /// </remarks>
        [HttpPut]
        [Permission("FacultyDuty.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateFacultyDutyDto request)
        {
            var command = new UpdateFacultyDutyRequest { UpdateFacultyDutyDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa nhiệm vụ của khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Permission("FacultyDuty.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<FacultyDuty> request)
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
