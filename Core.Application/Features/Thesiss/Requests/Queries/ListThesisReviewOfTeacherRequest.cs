using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Thesis;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using PeriodEntity = Core.Domain.Entities.RegistrationPeriod;

namespace Core.Application.Features.Thesiss.Requests.Queries
{
    public class ListThesisReviewOfTeacherRequest : ListBaseRequest<ThesisDto>
    {
        public bool? isGetIssuer { get; set; }

        public bool? isGetThesisInstructions { get; set; }

        public bool? isGetThesisReviews { get; set; }

        public bool? isGetThesisMajors { get; set; }

        public bool? isGetAll { get; set; }

        public int? periodId { get; set; }
    }

    public class ListThesisReviewOfTeacherValidator : AbstractValidator<ListThesisReviewOfTeacherRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListThesisReviewOfTeacherValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<ThesisDto>());

            RuleFor(x => x.periodId)
                .MustAsync(async (periodId, token) =>
                {
                    var exists = await _unitOfWork.Repository<PeriodEntity>()
                        .FirstOrDefaultAsync(x => x.Id == periodId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("periodId"));
        }
    }
}
