using Core.Application.Constants;
using Core.Application.Transform;
using FluentValidation;
using InvitationEntity = Core.Domain.Entities.Invitation;

namespace Core.Application.DTOs.Invitation.Validators
{
    public class SendInvitationDtoValidator : AbstractValidator<SendInvitationDto>
    {
        public SendInvitationDtoValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage(ValidatorTranform.Required("message"))
                .MaximumLength(CONSTANT_COMMON.MESSAGE).WithMessage(ValidatorTranform.MaximumLength("message", CONSTANT_COMMON.MESSAGE));

            RuleFor(x => x.Status)
                .Must(status => InvitationEntity.GetSatus().Any(x => x.Equals(status)))
                .WithMessage(ValidatorTranform.Must("gender", InvitationEntity.GetSatus()));
        }
    }
}
