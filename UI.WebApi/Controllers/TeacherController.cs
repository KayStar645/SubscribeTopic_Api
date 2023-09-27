﻿using Core.Application.DTOs.Teacher;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Teachers.Requests.Commands;
using Core.Application.Features.Teachers.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/teacher")]
    [ApiController]
    //[Authorize]
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
        public async Task<ActionResult<TeacherDto>> Get([FromQuery] DetailTeacherRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
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
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Teacher> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                var responses = Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                switch (ex)
                {
                    case NotFoundException:
                        responses = Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                        break;
                    case BadRequestException:
                        responses = Result<TeacherDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                        break;
                }
                return StatusCode(responses.Code, responses);
            }
        }

    }
}
