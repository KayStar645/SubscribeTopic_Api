using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.ReportSchedule;
using Core.Application.Features.Base.Requests.Queries;
using FluentValidation;

namespace Core.Application.Features.ReportSchedule.Requests.Queries
{
    public class ListReportScheduleRequest : ListBaseRequest<ReportScheduleDto>
    {
        public bool isGetThesis { get; set; }
        public bool isGetTeacher { get; set; }
    }

    public class ReportScheduleDtoValidator : AbstractValidator<ListReportScheduleRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportScheduleDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<ReportScheduleDto>());
        }
    }
}
