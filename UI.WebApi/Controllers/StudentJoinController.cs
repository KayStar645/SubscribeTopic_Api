﻿using Core.Application.DTOs.StudentJoin;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.StudentJoins.Requests.Commands;
using Core.Application.Features.StudentJoins.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/studentJoin")]
    [ApiController]
    [Authorize]
    public class StudentJoinController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentJoinController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sinh viên theo Khoa/Ngành/Chuyên ngành và Đợt
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - facultyId/industryId/majorId: required
        /// - periodId: null
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "StudentJoin.View")]
        public async Task<ActionResult<List<StudentJoinDto>>> Get([FromQuery] ListStudentJoinRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin đợt tham gia của sinh viên theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Authorize(Roles = "StudentJoin.View")]
        public async Task<ActionResult<StudentJoinDto>> Get([FromQuery] DetailStudentJoinRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm đợt tham gia của sinh viên
        /// </summary>
        /// <remarks>
        /// Ràng buộc:
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "StudentJoin.Create")]
        public async Task<ActionResult<StudentJoinDto>> Post([FromBody] CreateStudentJoinDto StudentJoinRequest)
        {
            var command = new CreateStudentJoinRequest { createStudentJoinDto = StudentJoinRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa đợt tham gia của sinh viên
        /// </summary>
        /// <remarks>
        /// Ràng buộc:
        /// </remarks>
        [HttpPut]
        [Authorize(Roles = "StudentJoin.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateStudentJoinDto StudentJoinRequest)
        {
            var command = new UpdateStudentJoinRequest { updateStudentJoinDto = StudentJoinRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa đợt tham gia của sinh viên
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Authorize(Roles = "StudentJoin.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<StudentJoin> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<StudentJoinDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<StudentJoinDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<StudentJoinDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
