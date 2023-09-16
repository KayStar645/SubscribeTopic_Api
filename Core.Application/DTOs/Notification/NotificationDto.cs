using Core.Application.DTOs.Faculty;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Notification
{
    public class NotificationDto : BaseDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public string? Describe { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }

        public FacultyDto? Faculty { get; set; }
    }
}
