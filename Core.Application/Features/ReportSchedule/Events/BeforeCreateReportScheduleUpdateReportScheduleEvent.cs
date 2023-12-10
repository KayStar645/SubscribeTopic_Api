using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using ReportScheduleEntity = Core.Domain.Entities.ReportSchedule;

namespace Core.Application.Features.ReportSchedule.Events
{
    public class BeforeCreateReportScheduleUpdateReportScheduleEvent : INotification
    {
        public ReportScheduleEntity _reportSchedule { get; set; }

        public IHttpContextAccessor _httpContextAccessor;

        public IUnitOfWork _unitOfWork;

        public BeforeCreateReportScheduleUpdateReportScheduleEvent(ReportScheduleEntity reportSchedule,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _reportSchedule = reportSchedule;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeCreateReportScheduleUpdateReportScheduleHandler : INotificationHandler<BeforeCreateReportScheduleUpdateReportScheduleEvent>
    {
        public async Task Handle(BeforeCreateReportScheduleUpdateReportScheduleEvent pEvent, CancellationToken cancellationToken)
        {
            var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

            if (userType == CLAIMS_VALUES.TYPE_TEACHER)
            {
                // Từ id của người dùng lấy ra id của giáo viên
                var teacher = await pEvent._unitOfWork.Repository<Teacher>()
                    .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

                pEvent._reportSchedule.TeacherId = teacher.Id;
            }
            else
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }
        }
    }

}
