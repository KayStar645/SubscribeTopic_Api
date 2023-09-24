using Core.Application.DTOs.Department;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Departments.Requests.Commands;
using Core.Application.Features.Departments.Requests.Queries;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/department")]
    [ApiController]
    //[Authorize]
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
        public async Task<ActionResult> Post([FromBody] CreateDepartmentDto request)
        {
            var command = new CreateDepartmentRequest { createDepartmentDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
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
        /// </remarks>
        [HttpPut]
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
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Department> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Error = ResponseTranform.ServerError });
            }
        }
    }
}
