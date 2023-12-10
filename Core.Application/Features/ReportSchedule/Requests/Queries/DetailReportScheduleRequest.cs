using Core.Application.DTOs.ReportSchedule;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.ReportSchedule.Requests.Queries
{
    public class DetailReportScheduleRequest : DetailBaseRequest, IRequest<Result<ReportScheduleDto>>
    {
        public bool isGetThesis { get; set; }
        public bool isGetTeacher { get; set; }
    }
}
