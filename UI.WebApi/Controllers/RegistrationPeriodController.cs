using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.RegistrationPeriods.Requests.Commands;
using Core.Application.Features.RegistrationPeriods.Requests.Queries;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/registrationPeriod")]
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
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpGet("detail")]
        public async Task<ActionResult<RegistrationPeriodDto>> Get([FromQuery] DetailRegistrationPeriodRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        public async Task<ActionResult<RegistrationPeriodDto>> Post([FromBody] CreateRegistrationPeriodDto periodRequest)
        {
            var command = new CreateRegistrationPeriodRequest { CreateRegistrationPeriodDto = periodRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromForm] UpdateRegistrationPeriodDto periodRequest)
        {
            var command = new UpdateRegistrationPeriodRequest { UpdateRegistrationPeriodDto = periodRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromBody] DeleteBaseRequest<RegistrationPeriod> request)
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