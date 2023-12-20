using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Councils.Events
{
    public class BeforeCreateCouncilUpdateCouncilEvent : INotification
    {
        public Council _council { get; set; }

        public IHttpContextAccessor _httpContextAccessor;

        public IUnitOfWork _unitOfWork;

        public BeforeCreateCouncilUpdateCouncilEvent(Council council,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _council = council;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeCreateCouncilUpdateCouncilHandler : INotificationHandler<BeforeCreateCouncilUpdateCouncilEvent>
    {
        public async Task Handle(BeforeCreateCouncilUpdateCouncilEvent pEvent, CancellationToken cancellationToken)
        {
            var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_TEACHER)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }

            // Từ id của người dùng lấy ra khoa của giảng viên
            var facultyId = await pEvent._unitOfWork.Repository<Teacher>()
                        .Query().Where(x => x.UserId == int.Parse(userId))
                        .Include(x => x.Department)
                        .Select(x => x.Department.FacultyId)
                        .FirstOrDefaultAsync();

            pEvent._council.FacultyId = facultyId;
        }
    }

}
