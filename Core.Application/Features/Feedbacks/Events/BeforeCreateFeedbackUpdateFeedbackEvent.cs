using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Features.Feedbacks.Events
{
    public class BeforeCreateFeedbackUpdateFeedbackEvent : INotification
    {
        public Feedback _feedback { get; set; }

        public IHttpContextAccessor _httpContextAccessor;

        public IUnitOfWork _unitOfWork;

        public BeforeCreateFeedbackUpdateFeedbackEvent(Feedback feedback,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _feedback = feedback;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeCreateThesisUpdateThesisHandler : INotificationHandler<BeforeCreateFeedbackUpdateFeedbackEvent>
    {
        public async Task Handle(BeforeCreateFeedbackUpdateFeedbackEvent pEvent, CancellationToken cancellationToken)
        {
            try
            {
                var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

                if (userType == CLAIMS_VALUES.TYPE_TEACHER)
                {

                    // Từ id của người dùng lấy ra id của giáo viên
                    var teacher = await pEvent._unitOfWork.Repository<Teacher>()
                        .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

                    pEvent._feedback.CommenterId = teacher.Id;
                }
                else
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }

            }
            catch (UnauthorizedException ex)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
