using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Group;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.Features.Groups.Requests.Queries
{
    public class ListGroupRequest : ListBaseRequest<GroupDto>
    {
        public bool? isGetLeader { get; set; }

        public bool? isGetGroupMe { get; set; }

        public int? facultyId { get; set; }
    }

    public class ListGroupDtoValidator : AbstractValidator<ListGroupRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListGroupDtoValidator(IUnitOfWork unitOfWork, bool? isGetGroupMe)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<GroupDto>());

            if(isGetGroupMe != true)
            {
                RuleFor(x => x.facultyId)
                    .MustAsync(async (facultyId, token) =>
                    {
                        var exists = await _unitOfWork.Repository<FacultyEntity>()
                            .FirstOrDefaultAsync(x => x.Id == facultyId);
                        return exists != null;
                    })
                    .WithMessage(id => ValidatorTransform.MustIn("facultyId"));
            }  
        }
    }
}
