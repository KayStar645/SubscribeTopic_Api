namespace Core.Application.DTOs.RegistrationPeriod
{
    public class CreateRegistrationPeriodDto : IRegistrationPeriodDto
    {
        public string? Semester { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public int? FacultyId { get; set; }
    }
}
