using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using DutyEntity = Core.Domain.Entities.Duty;
using ThesisEntity = Core.Domain.Entities.Thesis;

namespace Core.Application.DTOs.Thesis.Validators
{
    public class ChangeStatusThesisDtoValidator : AbstractValidator<ChangeStatusThesisDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChangeStatusThesisDtoValidator(IUnitOfWork unitOfWork, int pThesisId, string? pStatus)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(ValidatorTransform.Required("id"));

            if(pStatus == ThesisEntity.STATUS_APPROVE_REQUEST)
            {
                RuleFor(x => x.DutyId)
                    .MustAsync(async (id, token) =>
                    {
                        var exists = await _unitOfWork.Repository<DutyEntity>()
                                            .FirstOrDefaultAsync(x => x.Id == id &&
                                                x.Type == DutyEntity.TYPE_DEPARTMENT);
                        return exists != null;
                    })
                    .WithMessage(id => ValidatorTransform.NotExistsValueInTable("dutyId", "duty"));
            }

            RuleFor(x => x.Status)
                .MustAsync(async (status, token) =>
                {
                    var oldStatus = (await _unitOfWork.Repository<ThesisEntity>()
                                .FirstOrDefaultAsync(x => x.Id == pThesisId)).Status;

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
