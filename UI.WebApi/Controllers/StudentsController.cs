using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Student;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Students.Requests.Commands;
using Core.Application.Features.Students.Requests.Queries;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentDto>>> Get([FromQuery] ListStudentRequest<StudentDto> request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpGet("Detail")]
        public async Task<ActionResult<StudentDto>> Get([FromQuery] DetailStudentRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        public async Task<ActionResult<StudentDto>> Post([FromBody] CreateStudentDto studentRequest)
        {
            var command = new CreateStudentRequest { createStudentDto = studentRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateStudentDto studentRequest)
        {
            var command = new UpdateStudentRequest { updateStudentDto = studentRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteBaseRequest<Student> { Id = id };
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
