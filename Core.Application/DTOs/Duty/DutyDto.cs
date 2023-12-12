using Core.Application.DTOs.Common;
using Core.Application.DTOs.Department;
using Core.Application.DTOs.Faculty;
using Core.Application.DTOs.Teacher;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Duty
{
    public class DutyDto : BaseDto
    {
        public string? Name { get; set; }

        public string? Content { get; set; }

        public int? NumberOfThesis { get; set; }

        public DateTime? TimeEnd { get; set; }

        public List<FileDto>? Files { get; set; }

        public string? Type { get; set; }

        public int? FacultyId { get; set; }

        public FacultyDto? Faculty { get; set; }

        public int? DepartmentId { get; set; }

        public DepartmentDto? Department { get; set; }

        public int? TeacherId { get; set; }

        public TeacherDto? Teacher { get; set; }
    }
}
