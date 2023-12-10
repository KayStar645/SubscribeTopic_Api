using Core.Application.DTOs.Major;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Student
{
    public class FriendDto : BaseDto
    {
        // Đây chỉ là trạng thái của đợt đăng ký hiện tại
        public static string STATUS_NEW = "N"; // Không liên quan
        public static string STATUS_SEND = "S"; // Đã gửi lời mời
        public static string STATUS_APPROVE = "A"; // Đã vào nhóm

        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Class { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int? MajorId { get; set; }
        public MajorDto? Major { get; set; }
        public string? Status { get; set; }
        public int StudentJoinId { get; set; }
    }
}
