using Core.Application.DTOs.Group;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Teacher;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Thesis
{
    public class ThesisRegisteredDto : BaseDto
    {
        public string? InternalCode { get; set; }

        public string? Name { get; set; }

        public string? Summary { get; set; }

        public int? MinQuantity { get; set; }

        public int? MaxQuantity { get; set; }

        public bool? IsRegister { get; set; }

        public List<string>? Messages { get; set; }

        // Giảng viên hướng dẫn
        public List<TeacherDto>? ThesisInstructions { get; set; }

        // Chuyên ngành phù hợp
        public List<MajorDto>? ThesisMajors { get; set; }

        // Nhóm nào đăng ký đề tài này
        public GroupDto? GroupDto { get; set; }
    }
}
