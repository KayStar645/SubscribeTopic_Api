using Core.Application.DTOs.Group;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Student;
using Core.Application.DTOs.Teacher;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Thesis
{
    public class ThesisDto : BaseDto
    {
        public string? InternalCode { get; set; }

        public string? Name { get; set; }

        public int? ParentId { get; set; }

        public string? Summary { get; set; }

        public int? MinQuantity { get; set; }

        public int? MaxQuantity { get; set; }

        public string? Status { get; set; }

        public string? Type { get; set; }

        // Giảng viên ra đề
        public int? LecturerThesisId { get; set; }
        public TeacherDto? LecturerThesis { get; set; }

        // Sinh viên đề xuất
        public int? ProposedStudentId { get; set; }
        public StudentDto? ProposedStudent { get; set; }

        // Giảng viên hướng dẫn
        public List<TeacherDto>? ThesisInstructions { get; set; }

        // Giảng viên phản biện
        public List<TeacherDto>? ThesisReviews { get; set; }

        // Chuyên ngành phù hợp
        public List<MajorDto>? ThesisMajors { get; set; }

        // Nhóm nào đăng ký đề tài này
        public GroupDto? GroupDto { get; set; }
    }
}
