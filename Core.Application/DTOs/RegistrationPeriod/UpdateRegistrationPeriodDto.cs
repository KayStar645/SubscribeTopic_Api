using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.RegistrationPeriod
{
    public class UpdateRegistrationPeriodDto : BaseDto, IRegistrationPeriodDto
    {
        public int? Phase { get; set; }
        public string? Semester { get; set; }
        public string? Year { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public int? FacultyId { get; set; }

    }
}
