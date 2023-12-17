using Core.Application.DTOs.Teacher;

namespace Core.Application.DTOs.Point
{
    public class TeacherPointDto
    {
        public int? PointId { get; set; }

        public double? Score { get; set; }

        public TeacherDto? Teacher { get; set; }

        public string? Type { get; set; }
    }
}
