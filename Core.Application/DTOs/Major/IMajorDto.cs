namespace Core.Application.DTOs.Major
{
    public interface IMajorDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public int? FacultyId { get; set; }
    }
}
