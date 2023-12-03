using Core.Application.DTOs.Student;
using Core.Application.DTOs.Teacher;
using Core.Application.Models.Identity.Roles;

namespace Core.Application.Models.Identity.ViewModels
{
    public class UserVM
    {
        public int Id { get; set; }

        public string? UserName { get; set; }

        public string? Type { get; set; }

        public TeacherDto? Teacher { get; set; }

        public StudentDto? Student { get; set; }

        public List<RoleResult>? Roles { get; set; }

    }
}
