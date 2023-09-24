namespace Core.Application.DTOs.Industry
{
    public interface IIndustryDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public int? FacultyId { get; set; }
    }
}
