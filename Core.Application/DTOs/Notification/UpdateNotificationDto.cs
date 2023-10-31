using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Notification
{
    public class UpdateNotificationDto : BaseDto, INotificationDto
    {
        public int? FacultyId { get; set; }
        public string? Name { get; set; }
        public string? Describe { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public List<string>? Images { get; set; }
    }
}
