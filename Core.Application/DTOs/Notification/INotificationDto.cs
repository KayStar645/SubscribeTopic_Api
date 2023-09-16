namespace Core.Application.DTOs.Notification
{
    public interface INotificationDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public string? Describe { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
    }
}
