using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Duties.Events
{
    public class BeforeCreateDepartmentDutyUpdateDutyEvent : INotification
    {
        public Duty _duty { get; set; }

        public IHttpContextAccessor _httpContextAccessor;

        public IUnitOfWork _unitOfWork;

        public BeforeCreateDepartmentDutyUpdateDutyEvent(Duty duty,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _duty = duty;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeCreateDepartmentDutyUpdateDutyHandler : INotificationHandler<BeforeCreateDepartmentDutyUpdateDutyEvent>
    {
        public async Task Handle(BeforeCreateDepartmentDutyUpdateDutyEvent pEvent, CancellationToken cancellationToken)
        {
            var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;
            var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_TEACHER)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }

            // Từ id của người dùng lấy ra id của bộ môn
            var departmentId = await pEvent._unitOfWork.Repository<Teacher>()
                                    .Query()
                                    .Where(x => x.UserId == int.Parse(userId))
                                    .Select(x => x.DepartmentId)
                                    .FirstAsync();
            pEvent._duty.DepartmentId = departmentId;
        }
    }

}
