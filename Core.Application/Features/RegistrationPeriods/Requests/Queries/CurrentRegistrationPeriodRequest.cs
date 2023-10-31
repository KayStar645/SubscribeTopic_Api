namespace Core.Application.Features.RegistrationPeriods.Requests.Queries
{
    public class CurrentRegistrationPeriodRequest
    {
        public string? schoolYear { get; set; }
        public string? semester { get; set; }
        public int? facultyId { get; set; }
    }
}
