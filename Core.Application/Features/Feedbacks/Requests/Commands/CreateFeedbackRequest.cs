using Core.Application.DTOs.Feedback;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Feedbacks.Requests.Commands
{
    public class CreateFeedbackRequest : IRequest<Result<FeedbackDto>>
    {
        public CreateFeedbackDto? createFeedbackDto { get; set; }
    }
}
