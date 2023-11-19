using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Application.Transform;
using FluentValidation;
using InvitationEntity = Core.Domain.Entities.Invitation;

namespace Core.Application.DTOs.Invitation.Validators
{
    public class ChangeStatusInvitationDtoValidator : AbstractValidator<ChangeStatusThesisDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeStatusInvitationDtoValidator(IUnitOfWork unitOfWork, int pId)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));

            RuleFor(x => x.Status)
                .MustAsync(async (status, token) =>
                {
                    var oldStatus = (await _unitOfWork.Repository<InvitationEntity>().FirstOrDefaultAsync(x => x.Id == pId)).Status;

                    if (oldStatus == InvitationEntity.STATUS_SENT)
                    {
                        return status == InvitationEntity.STATUS_ACCEPT || status == InvitationEntity.STATUS_CANCEL;
                    }
                    return false;
                }).WithMessage(ValidatorTransform.MustIn("status"));
        }
    }
}
