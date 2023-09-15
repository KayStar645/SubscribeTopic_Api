namespace Core.Application.DTOs.Student
{
    public interface IStudentDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Class { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int? MajorId { get; set; }
    }
}
