using Core.Application.DTOs.Student;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Students.Requests.Commands;
using Core.Application.Features.Students.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/student")]
    [ApiController]
    //[Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách sinh viên theo khoa/ngành/chuyên ngành (Nên bổ sung khóa, lớp)
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - facultyId/industryId/majorId: required
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<List<StudentDto>>> Get([FromQuery] ListStudentRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin sinh viên theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        public async Task<ActionResult<StudentDto>> Get([FromQuery] DetailStudentRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm sinh viên
        /// </summary>
        /// <remarks>
        /// Ràng buộc:
        /// - Name: string, required, max(190)
        /// - Gender: string, in ["Nam", "Nữ", "Khác"]
        /// - DateOfBirth: DateTime, đủ 16 tuổi
        /// - PhoneNumber: string, lenght(10)
        /// - Email: string, email_format
        /// - Class: string, required
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<StudentDto>> Post([FromBody] CreateStudentDto studentRequest)
        {
            var command = new CreateStudentRequest { createStudentDto = studentRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa sinh viên
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// - Gender: string, in ["Nam", "Nữ", "Khác"]
        /// - DateOfBirth: DateTime, đủ 16 tuổi
        /// - PhoneNumber: string, lenght(10)
        /// - Email: string, email_format
        /// - Class: string, required
        /// </remarks>
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateStudentDto studentRequest)
        {
            var command = new UpdateStudentRequest { updateStudentDto = studentRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa sinh viên
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Student> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                var responses = Result<StudentDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                switch (ex)
                {
                    case NotFoundException:
                        responses = Result<StudentDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                        break;
                    case BadRequestException:
                        responses = Result<StudentDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                        break;
                }
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
