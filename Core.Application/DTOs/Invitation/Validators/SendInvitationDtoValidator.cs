using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using StudentJoinEntity = Core.Domain.Entities.StudentJoin;

namespace Core.Application.DTOs.Invitation.Validators
{
    public class SendInvitationDtoValidator : AbstractValidator<SendInvitationDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SendInvitationDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage(ValidatorTransform.Required("message"))
                .MaximumLength(CONSTANT_COMMON.MESSAGE).WithMessage(ValidatorTransform.MaximumLength("message", CONSTANT_COMMON.MESSAGE));

            RuleFor(x => x.StudentJoinId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<StudentJoinEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("facultyId", "faculties"));
        }
    }
}
