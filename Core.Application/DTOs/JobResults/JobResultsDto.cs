using Core.Application.DTOs.Job;
using Core.Application.DTOs.Student;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.JobResults
{
    public class JobResultsDto : BaseDto
    {
        public string? Files { get; set; }

        public int? StudentId { get; set; }

        public StudentDto? StudentBy { get; set; }

        public int? JobId { get; set; }

        public JobDto? ForJob { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
