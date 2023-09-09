using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Faculty
{
    public class FacultyDto : BaseDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
