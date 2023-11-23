using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.ThesisRegistrations.Event
{
    public class BeforeDeleteThesisRegistrationCheckRoleEvent : INotification
    {
        public ThesisRegistration _thesisRegistration { get; set; }
        public IHttpContextAccessor _httpContextAccessor;
        public IUnitOfWork _unitOfWork;

        public BeforeDeleteThesisRegistrationCheckRoleEvent(ThesisRegistration thesisRegistration,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _thesisRegistration = thesisRegistration;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }
    public class BeforeDeleteThesisRegistrationCheckRoleHandler : INotificationHandler<BeforeDeleteThesisRegistrationCheckRoleEvent>
    {
        public async Task Handle(BeforeDeleteThesisRegistrationCheckRoleEvent pEvent, CancellationToken cancellationToken)
        {
            var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var facultyId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.FacultyId)?.Value;

            // Chỉ cho đợt đăng ký đề tài của đợt hiện tại trong thời gian quy định
            var period = await pEvent._unitOfWork.Repository<RegistrationPeriod>()
                                .Query()
                                .Where(x => x.FacultyId == int.Parse(facultyId) && x.IsActive &&
                                            x.TimeStart <= DateTime.Now && DateTime.Now <= x.TimeEnd)
                                .FirstOrDefaultAsync();

            if (period == null)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }

            var group = await pEvent._unitOfWork.Repository<StudentJoin>()
                                    .Query()
                                    .Where(x => x.RegistrationPeriodId == period.Id &&
                                                x.Group.Leader.Student.UserId == int.Parse(userId))
                                .FirstOrDefaultAsync();

            // Chỉ cho trưởng nhóm hủy đăng ký
            if (group == null)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }

        }
    }

}
