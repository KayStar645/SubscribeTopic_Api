namespace Core.Application.DTOs.Duty
{
    public interface IDutyDto
    {
        public string? Name { get; set; }

        public string? Content { get; set; }

        public int? NumberOfThesis { get; set; }

        public DateTime? TimeEnd { get; set; }

        public List<string>? Files { get; set; }
    }
}
