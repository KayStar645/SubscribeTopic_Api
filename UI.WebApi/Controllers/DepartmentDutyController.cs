using Core.Application.DTOs.DepartmentDuty;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.DepartmentDuties.Requests.Commands;
using Core.Application.Features.DepartmentDuties.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/departmentDuty")]
    [ApiController]
    //[Authorize]
    public class DepartmentDutyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentDutyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách nhiệm vụ bộ môn giao cho giảng viên
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - departmentId/teacherId: required
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] ListDepartmentDutyRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin nhiệm vụ của bộ môn giao cho giảng viên theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        public async Task<ActionResult> Get([FromQuery] DetailDepartmentDutyRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm nhiệm vụ của bộ môn cho giảng viên
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
        public async Task<ActionResult> Post([FromBody] CreateDepartmentDutyDto request)
        {
            var command = new CreateDepartmentDutyRequest { CreateDepartmentDutyDto = request };
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
        /// Sửa nhiệm vụ của bộ môn
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
        public async Task<ActionResult> Put([FromBody] UpdateDepartmentDutyDto request)
        {
            var command = new UpdateDepartmentDutyRequest { UpdateDepartmentDutyDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa nhiệm vụ của bộ môn
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<DepartmentDuty> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<DepartmentDutyDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<DepartmentDutyDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<DepartmentDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
