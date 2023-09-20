using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Notification;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Notifications.Requests.Commands;
using Core.Application.Features.Notifications.Requests.Queries;
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
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] ListNotificationRequest<NotificationDto> request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpGet("Detail")]
        public async Task<ActionResult<NotificationDto>> Get([FromQuery] DetailNotificationRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> Post([FromBody] CreateNotificationDto request)
        {
            var command = new CreateNotificationRequest { createNotificationDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateNotificationDto request)
        {
            var command = new UpdateNotificationRequest { updateNotificationDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteBaseRequest<Notification> { Id = id };
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
