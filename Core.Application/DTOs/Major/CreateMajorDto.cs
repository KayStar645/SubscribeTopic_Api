namespace Core.Application.DTOs.Major
{
    public class CreateMajorDto : IMajorDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public int? IndustryId { get; set; }
    }
}
