using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.StudentJoin
{
    public class UpdateStudentJoinDto : BaseDto, IStudentJoinDto
    {
        public int? studentId { get; set; }
        public int? registrationPeriodId { get; set; }
    }
}
