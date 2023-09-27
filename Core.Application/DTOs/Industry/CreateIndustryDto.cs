namespace Core.Application.DTOs.Industry
{
    public class CreateIndustryDto : IIndustryDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public int? FacultyId { get; set; }
    }
}
