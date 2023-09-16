using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Notification
{
    internal class UpdateNotificationDto : BaseDto, INotificationDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public string? Describe { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
    }
}
