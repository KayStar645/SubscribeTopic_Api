using Core.Application.Contracts.Persistence;
using FluentValidation;

namespace Core.Application.DTOs.ReportSchedule.Validators
{
    public class UpdateReportScheduleDtoValidor : AbstractValidator<UpdateReportScheduleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateReportScheduleDtoValidor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ReportScheduleDtoValidor(_unitOfWork));
        }
    }
}
