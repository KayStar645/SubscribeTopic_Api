using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Points.Events
{
    public class BeforeUpdatePointUpdatePointEvent : INotification
    {
        public Point _point { get; set; }

        public IHttpContextAccessor _httpContextAccessor;

        public IUnitOfWork _unitOfWork;

        public BeforeUpdatePointUpdatePointEvent(Point point,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _point = point;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeUpdatePointUpdatePointHandler : INotificationHandler<BeforeUpdatePointUpdatePointEvent>
    {
        public async Task Handle(BeforeUpdatePointUpdatePointEvent pEvent, CancellationToken cancellationToken)
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
            // Đúng: phải là giảng viên hướng dẫn/Phản biện của sinh viên này
            pEvent._point.TeacherId = teacher?.Id;

            // Cập nhật lại Type là gvhd hay gvpb
            var isTeacherInstruct = await pEvent._unitOfWork.Repository<StudentJoin>()
                                    .Query()
                                    .AnyAsync(x => x.Group.ThesisRegistration.Thesis.ThesisInstructions
                                                .Any(x => x.TeacherId == teacher.Id));
            if(isTeacherInstruct == true)
            {
                pEvent._point.Type = Point.TYPE_INSTRUCTION;
                return;
            }

            var isTeacherReview = await pEvent._unitOfWork.Repository<StudentJoin>()
                                    .Query()
                                    .AnyAsync(x => x.Group.ThesisRegistration.Thesis.ThesisReviews
                                                .Any(x => x.TeacherId == teacher.Id));
            if (isTeacherReview == true)
            {
                pEvent._point.Type = Point.TYPE_REVIEW;
                return;
            }

            //throw new UnauthorizedException(StatusCodes.Status403Forbidden);
        }
    }

}
