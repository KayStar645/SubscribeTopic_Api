namespace Core.Application.DTOs.FacultyDuty
{
    public interface IFacultyDutyDto
    {
        public int? FacultyId { get; set; }
        public int? DepartmentId { get; set; }
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public int? NumberOfThesis { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public string? Image { get; set; }
        public string? File { get; set; }
    }
}
