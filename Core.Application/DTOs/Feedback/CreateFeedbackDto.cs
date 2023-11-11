namespace Core.Application.DTOs.Feedback
{
    public class CreateFeedbackDto
    {
        public string? Content { get; set; }

        public int? ThesisId { get; set; }
    }
}
