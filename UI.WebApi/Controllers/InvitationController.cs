using Core.Application.DTOs.Industry;
using Core.Application.DTOs.Invitation;
using Core.Application.Features.Industries.Requests.Queries;
using Core.Application.Features.Invitations.Request.Commands;
using Core.Application.Features.Invitations.Request.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UI.WebApi.Middleware;

namespace UI.WebApi.Controllers
{
    [Route("api/invitation")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvitationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách lời mời theo GroupId
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// GroupId: phải tồn tại trong bảng Group
        /// </remarks>
        [HttpGet]
        [Permission("Invitation.View")]
        public async Task<ActionResult<List<InvitationDto>>> Get([FromQuery] ListInvitationRequest pRequest)
        {
            var response = await _mediator.Send(pRequest);

            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Gửi mời mời vào nhóm
        /// </summary>
        /// <remarks>
        /// Ràng buộc: 
        /// - studentJoinId: phải tồn tại trong bảng StudentJoin
        /// - Chỉ sinh viên có trong đợt đăng ký hiện tại mới thực hiện được
        /// </remarks>
        [HttpPost]
        [Permission("Invitation.Create")]
        public async Task<ActionResult<InvitationDto>> Post([FromBody] SendInvitationDto pRequest)
        {
            var command = new SendInvitationRequest { sendInvitationDto = pRequest };
            var response = await _mediator.Send(command);

            return StatusCode(response.Code, response);
        }
    }
}
