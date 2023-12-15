using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Duties.Events
{
    public class BeforeCreateFacultyDutyUpdateDutyEvent : INotification
    {
        public Duty _duty { get; set; }

        public IHttpContextAccessor _httpContextAccessor;

        public IUnitOfWork _unitOfWork;

        public BeforeCreateFacultyDutyUpdateDutyEvent(Duty duty,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _duty = duty;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeCreateFacultyDutyUpdateDutyHandler : INotificationHandler<BeforeCreateFacultyDutyUpdateDutyEvent>
    {
        public async Task Handle(BeforeCreateFacultyDutyUpdateDutyEvent pEvent, CancellationToken cancellationToken)
        {
            var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;
            var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_TEACHER)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }

            // Từ id của người dùng lấy ra id của khoa
            var facultyId = await pEvent._unitOfWork.Repository<Teacher>()
                                    .Query()
                                    .Where(x => x.UserId == int.Parse(userId))
                                    .Include(x => x.Department)
                                    .Select(x => x.Department.FacultyId)
                                    .FirstAsync();
            pEvent._duty.FacultyId = facultyId;

            // Lấy đợt đăng ký hiện tại của khoa
            var periodCurrentId = await pEvent._unitOfWork.Repository<RegistrationPeriod>()
                                        .Query()
                                        .Where(x => x.FacultyId == facultyId && x.IsActive == true)
                                        .Select(x => x.Id)
                                        .FirstOrDefaultAsync();
            pEvent._duty.PeriodId = periodCurrentId;
        }
    }

}
