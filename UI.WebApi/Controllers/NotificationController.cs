using Core.Application.DTOs.Notification;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Application.Features.Notifications.Requests.Commands;
using Core.Application.Features.Notifications.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UI.WebApi.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách thông báo theo khoa hoặc không
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// facultyId: null
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Notification.View")]
        public async Task<ActionResult> Get([FromQuery] ListNotificationRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Lấy thông tin thông báo theo mã
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpGet("detail")]
        [Authorize(Roles = "Notification.View")]
        public async Task<ActionResult<NotificationDto>> Get([FromQuery] DetailNotificationRequest request)
        {
            var response = await _mediator.Send(request);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Thêm thông báo
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Name: string, required, max(190)
        /// - Describe: string, max(5000)
        /// - Image: string, url_format
        /// - Images: [string], url_format
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Notification.Create")]
        public async Task<ActionResult<NotificationDto>> Post([FromBody] CreateNotificationDto request)
        {
            var command = new CreateNotificationRequest { createNotificationDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Sửa thông báo
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// - Name: string, required, max(190)
        /// - Describe: string, max(5000)
        /// - Image: string, url_format
        /// - Images: [string], url_format
        /// </remarks>
        [HttpPut]
        [Authorize(Roles = "Notification.Update")]
        public async Task<ActionResult> Put([FromBody] UpdateNotificationDto request)
        {
            var command = new UpdateNotificationRequest { updateNotificationDto = request };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Xóa thông báo
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - Id: int, required
        /// </remarks>
        [HttpDelete]
        [Authorize(Roles = "Notification.Delete")]
        public async Task<ActionResult> Delete([FromQuery] DeleteBaseRequest<Notification> request)
        {
            try
            {
                var response = await _mediator.Send(request);
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            catch (NotFoundException ex)
            {
                var responses = Result<NotificationDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
                return StatusCode(responses.Code, responses);
            }
            catch (BadRequestException ex)
            {
                var responses = Result<NotificationDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
                return StatusCode(responses.Code, responses);
            }
            catch (Exception ex)
            {
                var responses = Result<NotificationDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
                return StatusCode(responses.Code, responses);
            }
        }
    }
}
