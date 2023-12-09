namespace Core.Application.DTOs.JobResults
{
    public class SubmitJobResultsDto
    {
        public List<string>? Files { get; set; }

        public int? JobId { get; set; }
    }
}
