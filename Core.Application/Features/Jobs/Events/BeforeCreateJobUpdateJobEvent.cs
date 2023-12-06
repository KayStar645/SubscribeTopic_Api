using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Features.Jobs.Events
{
    public class BeforeCreateJobUpdateJobEvent : INotification
    {
        public Job _job { get; set; }

        public IHttpContextAccessor _httpContextAccessor;

        public IUnitOfWork _unitOfWork;

        public BeforeCreateJobUpdateJobEvent(Job job,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _job = job;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeCreateJobUpdateJobHandler : INotificationHandler<BeforeCreateJobUpdateJobEvent>
    {
        public async Task Handle(BeforeCreateJobUpdateJobEvent pEvent, CancellationToken cancellationToken)
        {
            var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_TEACHER)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }
            // Từ id của người dùng lấy ra id của giáo viên
            var teacher = await pEvent._unitOfWork.Repository<Teacher>()
                .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

            pEvent._job.TeacherId = teacher.Id;

        }
    }

}