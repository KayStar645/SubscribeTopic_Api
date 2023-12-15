using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Point;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using GrooupEntity = Core.Domain.Entities.Group;

namespace Core.Application.Features.Points.Requests.Queries
{
    public class ListPointOfGroupRequest : ListBaseRequest<PointDto>
    {
        public bool? isGetGroupMe { get; set; }

        public int? groupId { get; set; }
    }

    public class ListPointOfGroupValidator : AbstractValidator<ListPointOfGroupRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListPointOfGroupValidator(IUnitOfWork unitOfWork, bool? isGetGroupMe)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<PointDto>());

            if(isGetGroupMe != true)
            {
                RuleFor(x => x.groupId)
                   .MustAsync(async (facultyId, token) =>
                   {
                       var exists = await _unitOfWork.Repository<GrooupEntity>()
                           .FirstOrDefaultAsync(x => x.Id == facultyId);
                       return exists != null;
                   })
                   .WithMessage(id => ValidatorTransform.MustIn("groupId"));
            }
        }
    }
}
