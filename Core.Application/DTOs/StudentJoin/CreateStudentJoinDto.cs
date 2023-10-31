namespace Core.Application.DTOs.StudentJoin
{
    public class CreateStudentJoinDto : IStudentJoinDto
    {
        public int? studentId { get; set; }
        public int? registrationPeriodId { get; set; }
    }
}
