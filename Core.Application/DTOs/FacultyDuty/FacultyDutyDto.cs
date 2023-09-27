using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.Teacher;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.FacultyDuty
{
    public class FacultyDutyDto : BaseDto
    {
        public int? FacultyId { get; set; }
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public string? NumberOfThesis { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public string? Image { get; set; }
        public string? File { get; set; }

        public FacultyDto? Faculty { get; set; }
    }
}
