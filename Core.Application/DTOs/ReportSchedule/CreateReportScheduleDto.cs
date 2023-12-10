namespace Core.Application.DTOs.ReportSchedule
{
    public class CreateReportScheduleDto : IReportScheduleDto
    {
        public DateTime? DateTime { get; set; }

        public string? Location { get; set; }

        public string? Type { get; set; }

        public int? ThesisId { get; set; }
    }
}
