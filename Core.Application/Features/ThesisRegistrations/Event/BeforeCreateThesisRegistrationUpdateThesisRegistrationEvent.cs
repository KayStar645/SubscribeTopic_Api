using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.ThesisRegistrations.Event
{
    public class BeforeCreateThesisRegistrationUpdateThesisRegistrationEvent : INotification
    {
        public ThesisRegistration _thesisRegistration { get; set; }
        public IHttpContextAccessor _httpContextAccessor;
        public IUnitOfWork _unitOfWork;

        public BeforeCreateThesisRegistrationUpdateThesisRegistrationEvent(ThesisRegistration thesisRegistration,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _thesisRegistration = thesisRegistration;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeCreateThesisRegistrationUpdateThesisRegistrationHandler : INotificationHandler<BeforeCreateThesisRegistrationUpdateThesisRegistrationEvent>
    {
        public async Task Handle(BeforeCreateThesisRegistrationUpdateThesisRegistrationEvent pEvent, CancellationToken cancellationToken)
        {
            var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

            // Chỉ cho sinh đăng đăng ký đề tài
            if (userType != CLAIMS_VALUES.TYPE_STUDENT)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }

            // Chỉ cho trưởng nhóm đăng ký


            var groupId = await pEvent._unitOfWork.Repository<Group>()
                                .Query()
                                .Include(x => x.Leader)
                                    .ThenInclude(x => x.Student)
                                .Where(x => x.Leader.Student.UserId == int.Parse(userId))
                                .Select(x => x.Id)
                                .FirstOrDefaultAsync();
            pEvent._thesisRegistration.GroupId = groupId;

        }
    }

}

