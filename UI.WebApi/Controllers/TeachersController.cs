using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Teacher;
using Core.Application.Exceptions;
using Core.Application.Features.Teachers.Requests.Commands;
using Core.Application.Features.Teachers.Requests.Queries;
using Core.Application.Transform;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeachersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TeacherDto>>> Get([FromQuery] ListDepartmentRequest<TeacherDto> request)
        {
            var validator = new ListBaseRequestValidator<TeacherDto>();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return BadRequest(errorMessages);
            }

            var response = await _mediator.Send(request);
            if (response.Succeeded)
            {
                return StatusCode(response.Code, response);
            }
            else
            {
                return StatusCode(response.Code, response);
            }
        }

        [HttpGet("Detail")]
        public async Task<ActionResult<TeacherDto>> Get([FromQuery] DetailTeacherRequest request)
        {
            var response = await _mediator.Send(request);

            if (response.Succeeded)
            {
                return StatusCode(response.Code, response);
            }
            else
            {
                return StatusCode(response.Code, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TeacherDto>> Post([FromBody] CreateTeacherDto teacherRequest)
        {
            var command = new CreateTeacherRequest { CreateTeacherDto = teacherRequest };
            var response = await _mediator.Send(command);

            if (response.Succeeded)
            {
                return StatusCode(response.Code, response);
            }
            else
            {
                return StatusCode(response.Code, response);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateTeacherDto teacherRequest)
        {
            var command = new UpdateTeacherRequest { UpdateTeacherDto = teacherRequest };
            var response = await _mediator.Send(command);
            if (response.Succeeded)
            {
                return StatusCode(response.Code, response);
            }
            else
            {
                return StatusCode(response.Code, response);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteTeacherRequest { Id = id };
                var response = await _mediator.Send(command);
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
