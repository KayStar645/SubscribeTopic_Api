using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.DTOs.Student;
using Core.Application.Exceptions;
using Core.Application.Features.RegistrationPeriods.Requests.Commands;
using Core.Application.Features.RegistrationPeriods.Requests.Queries;
using Core.Application.Features.Students.Requests.Commands;
using Core.Application.Features.Students.Requests.Queries;
using Core.Application.Transform;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegistrationPeriodController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegistrationPeriodController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<RegistrationPeriodDto>>> Get([FromQuery] ListRegistrationPeriodRequest<RegistrationPeriodDto> request)
        {
            var validator = new ListBaseRequestValidator<RegistrationPeriodDto>();
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

        [HttpGet("Detail")]
        public async Task<ActionResult<RegistrationPeriodDto>> Get([FromQuery] DetailRegistrationPeriodRequest request)
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
        public async Task<ActionResult<RegistrationPeriodDto>> Post([FromBody] CreateRegistrationPeriodDto periodRequest)
        {
            var command = new CreateRegistrationPeriodRequest { CreateRegistrationPeriodDto = periodRequest };
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
        public async Task<ActionResult> Put([FromBody] UpdateRegistrationPeriodDto periodRequest)
        {
            var command = new UpdateRegistrationPeriodRequest { UpdateRegistrationPeriodDto = periodRequest };
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
                var command = new DeleteRegistrationPeriodRequest { Id = id };
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