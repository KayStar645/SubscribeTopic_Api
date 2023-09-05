using Core.Application.DTOs.Teacher;
using Core.Application.Features.Teachers.Requests.Commands;
using Core.Application.Features.Teachers.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly IMediator _mediator;

        // Sửa lại respone phải ở ngoài này chớ
        public TeachersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TeacherDto>>> Get(int pageNumber = 1, int pageSize = 10)
        {
            var teachers = await _mediator.Send(
                    new GetTeacherListRequest() 
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize 
                    });
            return Ok(teachers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> Get(int id)
        {
            var teacher = await _mediator.Send(new GetTeacherDetailRequest { Id = id });
            return Ok(teacher);
        }

        [HttpPost]
        public async Task<ActionResult<BaseCommandResponse>> Post([FromBody] CreateTeacherDto teacherRequest)
        {
            var command = new CreateTeacherCommand { TeacherDto = teacherRequest };
            var reponse = await _mediator.Send(command);
            return Ok(reponse);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateTeacherDto teacherRequest)
        {
            var command = new UpdateTeacherCommand { UpdateTeacherDto = teacherRequest };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteTeacherCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

        
    }
}
