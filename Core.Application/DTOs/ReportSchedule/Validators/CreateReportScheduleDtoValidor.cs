using Core.Application.Contracts.Persistence;
using Core.Application.Transform;
using FluentValidation;
using ThesisEntity = Core.Domain.Entities.Thesis;

namespace Core.Application.DTOs.ReportSchedule.Validators
{
    public class CreateReportScheduleDtoValidor : AbstractValidator<CreateReportScheduleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateReportScheduleDtoValidor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ReportScheduleDtoValidor(_unitOfWork));

            RuleFor(x => x.ThesisId)
                .MustAsync(async (id, token) =>
                {
                    var exists = await _unitOfWork.Repository<ThesisEntity>().GetByIdAsync(id);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.NotExistsValueInTable("thesisId", "thesis"));
        }
    }
}
