using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.ThesisRegistration;
using Core.Application.Features.ThesisRegistrations.Requests.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThesisRegistrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ThesisRegistrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Thêm khoa
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - ThesisId: mã đề tài hợp lệ và có trạng thái A
        /// </remarks>
        [HttpPost]
        //[Permission("ThesisRegistration.Create")]
        public async Task<ActionResult<FacultyDto>> Post([FromBody] CreateThesisRegistrationDto pRequest)
        {
            var command = new CreateThesisRegistrationRequest { createThesisRegistrationDto = pRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }
    }
}
