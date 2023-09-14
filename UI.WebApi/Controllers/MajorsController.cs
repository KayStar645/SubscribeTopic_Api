using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Major;
using Core.Application.Features.Majors.Requests.Commands;
using Core.Application.Features.Majors.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MajorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MajorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<MajorDto>>> Get([FromQuery] ListMajorRequest<MajorDto> request)
        {
            var validator = new ListBaseRequestValidator<MajorDto>();
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

        [HttpPost]
        public async Task<ActionResult<MajorDto>> Post([FromBody] CreateMajorDto request)
        {
            var command = new CreateMajorRequest { CreateMajorDto = request };
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
    }
}
