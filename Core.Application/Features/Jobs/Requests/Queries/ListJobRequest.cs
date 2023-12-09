using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Job;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;

namespace Core.Application.Features.Jobs.Requests.Queries
{
    public class ListJobRequest : ListBaseRequest<JobDto>
    {
        public bool? isGetTeacher { get; set; }

        public bool? isGetThesis { get; set; }

        public int? thesisId { get; set; }
    }

    public class ListJobDtoValidator : AbstractValidator<ListJobRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListJobDtoValidator(IUnitOfWork unitOfWork, int? thesisId)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<JobDto>());

            RuleFor(x => x.thesisId)
                .MustAsync(async (thesisId, token) =>
                {
                    if(thesisId != null)
                    {
                        var exists = await _unitOfWork.Repository<ThesisEntity>()
                        .FirstOrDefaultAsync(x => x.Id == thesisId);
                        return exists != null;
                    }    
                    return false;
                })
                .WithMessage(id => ValidatorTransform.MustIn("thesisId"));
        }
    }
}
