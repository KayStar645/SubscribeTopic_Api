using Core.Application.DTOs.ReportSchedule;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.ReportSchedule.Requests.Commands
{
    public class UpdateReportScheduleRequest : IRequest<Result<ReportScheduleDto>>
    {
        public UpdateReportScheduleDto? updateReportScheduleDto { get; set; }
    }
}
