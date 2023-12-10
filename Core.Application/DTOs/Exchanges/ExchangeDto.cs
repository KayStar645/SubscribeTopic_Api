using Core.Application.DTOs.Student;
using Core.Application.DTOs.Teacher;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Exchanges
{
    public class ExchangeDto : BaseDto
    {
        public string? Content { get; set; }

        public int? StudentId { get; set; }

        public StudentDto? Student { get; set; }

        public int? TeacherId { get; set; }

        public TeacherDto? Teacher { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? JobId { get; set; }
    }
}
