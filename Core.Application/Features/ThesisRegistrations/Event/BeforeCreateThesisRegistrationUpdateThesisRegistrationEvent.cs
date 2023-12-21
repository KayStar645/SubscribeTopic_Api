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
            var facultyId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.FacultyId)?.Value;
            var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_STUDENT)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }

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

            var student = await pEvent._unitOfWork.Repository<Student>()
                .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

            var groupId = await pEvent._unitOfWork.Repository<StudentJoin>()
                                    .Query()
                                    .Where(x => x.RegistrationPeriodId == period.Id &&
                                                x.StudentId == student.Id)
                                    .Select(x => x.GroupId)
                                .FirstOrDefaultAsync();

            // Chỉ cho trưởng nhóm đăng ký
            if (groupId == null)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }

            // Chỉ cho đăng ký đề tài đã duyệt
            var thesis = await pEvent._unitOfWork.Repository<Thesis>()
                                    .FirstOrDefaultAsync(x => x.Id == pEvent._thesisRegistration.ThesisId &&
                                                              x.Status == Thesis.STATUS_APPROVED);
            if (thesis == null)
            {
                throw new BadRequestException("Đề tài đăng ký không hợp lệ!");
            }

            // 1 nhóm chỉ được đăng ký 1 đề tài
            var exists = await pEvent._unitOfWork.Repository<ThesisRegistration>()
                                .AnyAsync(x => x.GroupId == groupId);
            if (exists)
            {
                throw new BadRequestException("Một nhóm chỉ được đăng ký 1 đề tài!");
            }

            pEvent._thesisRegistration.GroupId = groupId;

        }
    }

}

