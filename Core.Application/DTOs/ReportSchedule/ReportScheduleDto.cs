using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Thesis;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.ReportSchedule
{
    public class ReportScheduleDto : BaseDto
    {
        public DateTime? TimeStart { get; set; }

        public DateTime? TimeEnd { get; set; }

        public string? Location { get; set; }

        public string? Type { get; set; }

        public int? ThesisId { get; set; }

        public ThesisDto? Thesis { get; set; }

        public int? TeacherId { get; set; }

        public TeacherDto? Teacher { get; set; }
    }
}
