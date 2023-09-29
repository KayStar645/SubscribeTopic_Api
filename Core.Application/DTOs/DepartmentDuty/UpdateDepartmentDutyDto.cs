using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.DepartmentDuty
{
    public class UpdateDepartmentDutyDto : BaseDto, IDepartmentDutyDto
    {
        public int? DepartmentId { get; set; }
        public int? TeacherId { get; set; }
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public string? NumberOfThesis { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public string? Image { get; set; }
        public string? File { get; set; }
    }
}
