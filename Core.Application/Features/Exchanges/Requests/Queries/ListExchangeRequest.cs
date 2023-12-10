using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Exchanges;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using JobEntity = Core.Domain.Entities.Job;

namespace Core.Application.Features.Exchanges.Requests.Queries
{
    public class ListExchangeRequest : ListBaseRequest<ExchangeDto>
    {
        public int? jobId { get; set; }
    }

    public class ListExchangeDtoValidator : AbstractValidator<ListExchangeRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListExchangeDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<ExchangeDto>());

            RuleFor(x => x.jobId)
                .MustAsync(async (jobId, token) =>
                {
                    var exists = await _unitOfWork.Repository<JobEntity>()
                        .FirstOrDefaultAsync(x => x.Id == jobId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("jobId"));
        }
    }
}
