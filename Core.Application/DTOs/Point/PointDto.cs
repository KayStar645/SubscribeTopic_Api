using Core.Application.DTOs.StudentJoin;
using Core.Application.DTOs.Teacher;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Point
{
    public class PointDto : BaseDto
    {
        public double? Scores { get; set; }

        public string? Type { get; set; }

        public int? StudentJoinId { get; set; }

        public StudentJoinDto? StudentJoin { get; set; }

        public int? TeacherId { get; set; }

        public TeacherDto? Teacher { get; set; }
    }
}
