using Core.Application.DTOs.Teacher;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Teachers.Requests.Commands;
using Core.Application.Features.Teachers.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/teacher")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeacherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách giảng viên theo Khoa/Bộ môn
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - facultyId/DepartmentId: required
        /// </remarks>
        [HttpGet]
        [Permission("Teacher.View")]
        public async Task<ActionResult<List<TeacherDto>>> Get([FromQuery] ListTeacherRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin giảng viên theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Permission("Teacher.View")]
        public async Task<ActionResult<TeacherDto>> Get([FromQuery] DetailTeacherRequest request)
        {
            var response = await _mediator.Send(request);

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
        /// Thêm giảng viên
        /// </summary>
        /// <remarks>
        /// Ràng buộc:
        /// - Name: string, required, max(190)
        /// - Gender: string, in ["Nam", "Nữ", "Khác"]
        /// - DateOfBirth: DateTime, đủ 16 tuổi
        /// - PhoneNumber: string, lenght(10)
        /// - Email: string, email_format
        /// - AcademicTitle: string, in ["Cử nhân", "Kỹ sư", "Cao học", "Thạc sĩ", "Tiến sĩ"]
        /// - Degree: string, Nếu AcademicTitle = "Tiến sĩ" -> in ["Phó giáo sư", "Giáo sư"], nếu không -> null
        /// - Type: string, in ["M", "L"]
        /// </remarks>
        [HttpPost]
        [Permission("Teacher.Create")]
        public async Task<ActionResult<TeacherDto>> Post([FromBody] CreateTeacherDto teacherRequest)
        {
            var command = new CreateTeacherRequest { createTeacherDto = teacherRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa giảng viên
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// - Gender: string, in ["Nam", "Nữ", "Khác"]
        /// - DateOfBirth: DateTime, đủ 16 tuổi
        /// - PhoneNumber: string, lenght(10)
        /// - Email: string, email_format
        /// - AcademicTitle: string, in ["Cử nhân", "Kỹ sư", "Cao học", "Thạc sĩ", "Tiến sĩ"]
        /// - Degree: string, Nếu AcademicTitle = "Tiến sĩ" -> in ["Phó giáo sư", "Giáo sư"], nếu không -> null
        /// - Type: string, in ["M", "L"]
        /// </remarks>
        [HttpPut]
        [Permission("Teacher.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateTeacherDto teacherRequest)
        {
            var command = new UpdateTeacherRequest { updateTeacherDto = teacherRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa giảng viên
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Permission("Teacher.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Teacher> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }

    }
}
