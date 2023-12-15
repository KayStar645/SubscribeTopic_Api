namespace Core.Application.DTOs.Duty.Faculty
{
    public class CreateDepartmentDutyDto : IDutyDto
    {
        public string? Name { get; set; }

        public string? Content { get; set; }

        public int? NumberOfThesis { get; set; }

        public DateTime? TimeEnd { get; set; }

        public List<string>? Files { get; set; }

        public int? TeacherId { get; set; }

        public int? DutyId { get; set; }
    }
}
