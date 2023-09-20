using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Faculty;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Faculties.Requests.Commands;
using Core.Application.Features.Faculties.Requests.Queries;
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
    public class FacultiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FacultiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<FacultyDto>>> Get([FromQuery] ListFacultyRequest<FacultyDto> request)
        {
            var response = await _mediator.Send(request);


            return StatusCode(response.Code, response);
        }

        [HttpGet("Detail")]
        public async Task<ActionResult<FacultyDto>> Get([FromQuery] DetailFacultyRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        public async Task<ActionResult<FacultyDto>> Post([FromBody] CreateFacultyDto FacultyRequest)
        {
            var command = new CreateFacultyRequest { createFacultyDto = FacultyRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateFacultyDto FacultyRequest)
        {
            var command = new UpdateFacultyRequest { updateFacultyDto = FacultyRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteBaseRequest<Faculty> { Id = id };
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
