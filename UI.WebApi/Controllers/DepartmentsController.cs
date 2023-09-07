using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Department;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Features.Departments.Requests.Commands;
using Core.Application.Features.Departments.Requests.Queries;
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
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DepartmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] ListBaseRequest<DepartmentDto> request)
        {
            var validator = new ListBaseRequestValidator<DepartmentDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
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

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var response = await _mediator.Send(new DetailDepartmentRequest { Id = id });

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
        public async Task<ActionResult> Post([FromBody] CreateDepartmentDto request)
        {
            var command = new CreateDepartmentCommand { CreateDepartmentDto = request };
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
        public async Task<ActionResult> Put([FromBody] UpdateDepartmentDto request)
        {
            var command = new UpdateDepartmentCommand { UpdateDepartmentDto = request };
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
                var command = new DeleteDepartmentCommand { Id = id };
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
