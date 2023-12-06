using Core.Application.DTOs.Common;
using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Thesis;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Job
{
    public class JobDto : BaseDto
    {
        public string? Name { get; set; }

        public string? Instructions { get; set; }

        public DateTime? Due { get; set; }

        public List<FileDto>? Files { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? TeacherId { get; set; }

        public TeacherDto? TeacherBy { get; set; }

        public int? ThesisId { get; set; }

        public ThesisDto? ForThesis { get; set; }
    }
}
