namespace Core.Application.DTOs.ReportSchedule
{
    public interface IReportScheduleDto
    {
        public DateTime? DateTime { get; set; }

        public string? Location { get; set; }

        public string? Type { get; set; }
    }
}
