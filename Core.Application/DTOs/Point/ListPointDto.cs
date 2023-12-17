using Core.Application.DTOs.StudentJoin;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Point
{
    public class ListPointDto : BaseDto
    {
        public double? InstructionScore { get; set; }

        public double? ViewScore { get; set; }

        public double? AverageScore { get; set; }

        public StudentJoinDto? StudentJoin { get; set; }
    }
}
