using Core.Application.DTOs.Teacher;

namespace Core.Application.DTOs.Commissioner
{
    public class CommissionerDto
    {
        public string? Position { get; set; }

        public int? TeacherId { get; set; }

        public TeacherDto? Teacher { get; set; }
    }
}
