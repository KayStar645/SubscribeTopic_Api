namespace Core.Application.DTOs.StudentJoin
{
    public class CreateStudentJoinDto
    {
        public List<int>? studentIds { get; set; }
        public int? registrationPeriodId { get; set; }
    }
}
