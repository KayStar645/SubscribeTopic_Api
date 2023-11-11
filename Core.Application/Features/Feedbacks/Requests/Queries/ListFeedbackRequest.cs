using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Feedback;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.Features.Feedbacks.Requests.Queries
{
    public class ListFeedbackRequest : ListBaseRequest<FeedbackDto>
    {
        public int? thesisId { get; set; }
    }

    public class ListFeedbackDtoValidator : AbstractValidator<ListFeedbackRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListFeedbackDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<FeedbackDto>());

            RuleFor(x => x.thesisId)
                .MustAsync(async (thesisId, token) =>
                {
                    var exists = await _unitOfWork.Repository<ThesisEntity>()
                        .FirstOrDefaultAsync(x => x.Id == thesisId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("thesisId"));
        }
    }
}
