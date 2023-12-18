using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Point;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;

namespace Core.Application.Features.Points.Requests.Queries
{
    public class ListPointOfThesisRequest : ListBaseRequest<ThesisPointDto>
    {
        public bool? isGetPointMe { get; set; }

        public int? thesisId { get; set; }
    }

    public class ListPointOfThesisValidator : AbstractValidator<ListPointOfThesisRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListPointOfThesisValidator(IUnitOfWork unitOfWork, bool? isGetPointMe)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<ThesisPointDto>());

            if (isGetPointMe != true)
            {
                RuleFor(x => x.thesisId)
                   .MustAsync(async (facultyId, token) =>
                   {
                       var exists = await _unitOfWork.Repository<ThesisEntity>()
                           .FirstOrDefaultAsync(x => x.Id == facultyId);
                       return exists != null;
                   })
                   .WithMessage(id => ValidatorTransform.MustIn("thesisId"));
            }
        }
    }
}
