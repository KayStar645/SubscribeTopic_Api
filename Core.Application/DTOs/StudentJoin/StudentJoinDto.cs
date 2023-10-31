using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.DTOs.Student;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.StudentJoin
{
    public class StudentJoinDto : BaseDto
    {
        public int? studentId { get; set; }
        public int? registrationPeriodId { get; set; }
        public double? score { get; set; }
        public StudentDto? student { get; set; }
        public RegistrationPeriodDto? registrationPeriod { get; set; }
    }
}
