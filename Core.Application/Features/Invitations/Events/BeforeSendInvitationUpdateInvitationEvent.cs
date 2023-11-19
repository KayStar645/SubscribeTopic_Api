using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Invitations.Events
{
    public class BeforeSendInvitationUpdateInvitationEvent : INotification
    {
        public Invitation _invitation { get; set; }

        public IHttpContextAccessor _httpContextAccessor;

        public IUnitOfWork _unitOfWork;

        public BeforeSendInvitationUpdateInvitationEvent(Invitation invitation,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _invitation = invitation;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeSendInvitationUpdateInvitationHandler : INotificationHandler<BeforeSendInvitationUpdateInvitationEvent>
    {
        public async Task Handle(BeforeSendInvitationUpdateInvitationEvent pEvent, CancellationToken cancellationToken)
        {
            var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_STUDENT)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }
            pEvent._invitation.Status = Invitation.STATUS_SENT;

            // Đợt đăng ký hiện tại
            var period = await pEvent._unitOfWork.Repository<RegistrationPeriod>()
                                     .FirstOrDefaultAsync(x => x.TimeStart <= DateTime.Now && DateTime.Now <= x.TimeEnd);

            if (period == null)
            {
                throw new BadRequestException("Bạn không có trong đợt đăng ký đề tài hiện tại!");
            }

            // Sinh viên tham gia đợt đăng ký
            var studentJoin = await pEvent._unitOfWork.Repository<StudentJoin>()
                                .GetAllInclude()
                                .Include(x => x.Student)
                                .FirstOrDefaultAsync(x => x.Student.UserId == int.Parse(userId) &&
                                                          x.RegistrationPeriodId == period.Id);

            if(studentJoin?.GroupId == null)
            {
                throw new BadRequestException("Bạn không có trong đợt đăng ký đề tài hiện tại!");
            }
            pEvent._invitation.GroupId = studentJoin?.GroupId;
        }
    }
}
