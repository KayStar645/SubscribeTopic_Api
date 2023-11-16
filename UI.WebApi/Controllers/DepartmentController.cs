using Core.Application.DTOs.Department;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Departments.Requests.Commands;
using Core.Application.Features.Departments.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách bộ môn theo khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - facultyId: required
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Department.View")]
        public async Task<ActionResult> Get([FromQuery] ListDepartmentRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin bộ môn theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Authorize(Roles = "Department.View")]
        public async Task<ActionResult> Get([FromQuery] DetailDepartmentRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm bộ môn
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Name: string, required, max(190)
        /// - PhoneNumber: string, length(10)
        /// - Email: string, email_format
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Department.Create")]
        public async Task<ActionResult> Post([FromBody] CreateDepartmentDto request)
        {
            var command = new CreateDepartmentRequest { createDepartmentDto = request };
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
        /// Sửa bộ môn
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// - PhoneNumber: string, length(10)
        /// - Email: string, email_format
        /// - HeadDepartment_TeacherId: Giảng viên của Bộ môn
        /// </remarks>
        [HttpPut]
        [Authorize(Roles = "Department.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateDepartmentDto request)
        {
            var command = new UpdateDepartmentRequest { updateDepartmentDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa bộ môn
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Authorize(Roles = "Department.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Department> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<DepartmentDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<DepartmentDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<DepartmentDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
