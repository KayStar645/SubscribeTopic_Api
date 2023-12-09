using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Invitation;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using GroupEntity = Core.Domain.Entities.Group;

namespace Core.Application.Features.Invitations.Requests.Queries
{
    public class ListInvitationRequest : ListBaseRequest<InvitationDto>
    {
        public bool isGetGroup { get; set; }

        public bool isGetStudentJoin { get; set; }

        public int? groupId { get; set; }
    }

    public class ListInvitationDtoValidator : AbstractValidator<ListInvitationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListInvitationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<InvitationDto>());

            RuleFor(x => x.groupId)
                .MustAsync(async (groupId, token) =>
                {
                    var exists = await _unitOfWork.Repository<GroupEntity>()
                        .FirstOrDefaultAsync(x => x.Id == groupId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("groupId"));
        }
    }
}
