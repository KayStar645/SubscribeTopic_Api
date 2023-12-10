using Core.Application.DTOs.ReportSchedule;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.ReportSchedule.Requests.Commands
{
    public class CreateReportScheduleRequest : IRequest<Result<ReportScheduleDto>>
    {
        public CreateReportScheduleDto? createReportScheduleDto { get; set; }
    }
}
