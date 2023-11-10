using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;

namespace Core.Application.DTOs.Thesis.Validators
{
    public class ChangeStatusThesisDtoValidator : AbstractValidator<ChangeStatusThesisDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeStatusThesisDtoValidator(IUnitOfWork unitOfWork, int pId)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));

            RuleFor(x => x.Status)
                .MustAsync(async (status, token) =>
                {
                    var oldStatus = (await _unitOfWork.Repository<ThesisEntity>().FirstOrDefaultAsync(x => x.Id == pId)).Status;

                    if(oldStatus == ThesisEntity.STATUS_DRAFT)
                    {
                        return status == ThesisEntity.STATUS_APPROVE_REQUEST;
                    }
                    if(oldStatus == ThesisEntity.STATUS_APPROVE_REQUEST)
                    {
                        return ThesisEntity.GetSatus().Contains(status);
                    }

                    return false;
                }).WithMessage(ValidatorTransform.MustIn("status"));
        }

    }
}
