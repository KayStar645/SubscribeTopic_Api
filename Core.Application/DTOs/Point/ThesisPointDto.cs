using Core.Application.DTOs.StudentJoin;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Point
{
    public class ThesisPointDto : BaseDto
    {
        public List<TeacherPointDto>? Scores { get; set; }

        public double? AverageScore { get; set; }

        public StudentJoinDto? StudentJoin { get; set; }
    }
}
