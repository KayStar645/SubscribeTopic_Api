namespace Core.Application.DTOs.Duty
{
    public class CreateDutyDto : IDutyDto
    {
        public string? Name { get; set; }

        public string? Content { get; set; }

        public int? NumberOfThesis { get; set; }

        public DateTime? TimeEnd { get; set; }

        public int? PeriodId { get; set; }

        public List<string>? Files { get; set; }

        public string? Type { get; set; }

        public int? DepartmentId { get; set; }

        public int? TeacherId { get; set; }
    }
}
