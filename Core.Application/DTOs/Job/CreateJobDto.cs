namespace Core.Application.DTOs.Job
{
    public class CreateJobDto : IJobDto
    {
        public int? ThesisId { get; set; }

        public string? Name { get; set; }

        public string? Instructions { get; set; }

        public DateTime? Due { get; set; }

        public List<string>? Files { get; set; }
    }
}
