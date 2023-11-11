using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Thesis;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Feedback
{
    public class FeedbackDto : BaseDto
    {
        public string? Content { get; set; }

        public DateTime? DateCreated { get; set; }

        public int? CommenterId { get; set; }

        public TeacherDto? Commenter { get; set; }

        public int? ThesisId { get; set; }

        public ThesisDto? Thesis { get; set; }
    }
}
