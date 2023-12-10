using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.ReportSchedule
{
    public class UpdateReportScheduleDto : BaseDto, IReportScheduleDto
    {
        public DateTime? DateTime { get; set; }

        public string? Location { get; set; }

        public string? Type { get; set; }
    }
}
