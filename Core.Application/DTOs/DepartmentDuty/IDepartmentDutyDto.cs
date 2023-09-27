namespace Core.Application.DTOs.DepartmentDuty
{
    public interface IDepartmentDutyDto
    {
        public int? DepartmentId { get; set; }
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public string? NumberOfThesis { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public string? Image { get; set; }
        public string? File { get; set; }
    }
}
