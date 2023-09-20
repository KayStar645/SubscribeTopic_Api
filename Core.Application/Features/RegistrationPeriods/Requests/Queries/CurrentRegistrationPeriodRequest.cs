namespace Core.Application.Features.RegistrationPeriods.Requests.Queries
{
    public class CurrentRegistrationPeriodRequest
    {
        public string? SchoolYear { get; set; }
        public string? Semester { get; set; }
        public int? FacultyId { get; set; }
    }
}
