using Core.Application.DTOs.Teacher;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Teachers.Requests.Commands;
using Core.Application.Features.Teachers.Requests.Queries;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/teacher")]
    [ApiController]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeacherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TeacherDto>>> Get([FromQuery] ListDepartmentRequest<TeacherDto> request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpGet("detail")]
        public async Task<ActionResult<TeacherDto>> Get([FromQuery] DetailTeacherRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        public async Task<ActionResult<TeacherDto>> Post([FromBody] CreateTeacherDto teacherRequest)
        {
            var command = new CreateTeacherRequest { createTeacherDto = teacherRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateTeacherDto teacherRequest)
        {
            var command = new UpdateTeacherRequest { updateTeacherDto = teacherRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromForm] DeleteBaseRequest<Teacher> request)
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
