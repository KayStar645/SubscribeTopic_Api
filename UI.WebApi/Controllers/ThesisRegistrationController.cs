using Core.Application.DTOs.ThesisRegistration;
using Core.Application.Exceptions;
using Core.Application.Features.ThesisRegistrations.Requests.Commands;
using Core.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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
        /// Đăng ký đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - ThesisId: mã đề tài hợp lệ và có trạng thái A
        /// </remarks>
        [HttpPost]
        [Permission("ThesisRegistration.Create")]
        public async Task<ActionResult> Post([FromBody] CreateThesisRegistrationDto pRequest)
        {
            var command = new CreateThesisRegistrationRequest { createThesisRegistrationDto = pRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Hủy đăng ký đề tài
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Permission("ThesisRegistration.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteThesisRegistrationRequest request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (UnauthorizedException ex)
            {
                var responses = Result<ThesisRegistrationDto>.Failure(ex.Message, (int)HttpStatusCode.Unauthorized);
                return StatusCode(responses.Code, responses);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<ThesisRegistrationDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<ThesisRegistrationDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<ThesisRegistrationDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
