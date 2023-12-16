namespace Core.Application.DTOs.ReportSchedule
{
    public interface IReportScheduleDto
    {
        public DateTime? TimeStart { get; set; }

        public DateTime? TimeEnd { get; set; }

        public string? Location { get; set; }

        public string? Type { get; set; }
    }
}
