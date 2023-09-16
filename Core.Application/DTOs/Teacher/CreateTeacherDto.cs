using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.DTOs.Teacher
{
    public class CreateTeacherDto : ITeacherDto
    {
        public string? InternalCode { get; set; }
        public int? DepartmentId { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AcademicTitle { get; set; }
        public string? Degree { get; set; }
        public string? Type { get; set; } = TeacherEntity.TYPE_TEACHER_LECTURERS;
    }
}
