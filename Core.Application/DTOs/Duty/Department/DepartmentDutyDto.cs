using Core.Application.DTOs.Common;
using Core.Application.DTOs.Department;
using Core.Application.DTOs.Teacher;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Duty.Faculty
{
    public class DepartmentDutyDto : BaseDto
    {
        public string? Name { get; set; }

        public string? Content { get; set; }

        public int? NumberOfThesis { get; set; }

        public DateTime? TimeEnd { get; set; }

        public List<FileDto>? Files { get; set; }

        public int? DepartmentId { get; set; }

        public DepartmentDto? Department { get; set; }

        public int? TeacherId { get; set; }

        public TeacherDto? Teacher { get; set; }

        public int? DutyId { get; set; }

        public FacultyDutyDto? ForDuty { get; set; }

        public int? NumberThesisComplete { get; set; }
    }
}
