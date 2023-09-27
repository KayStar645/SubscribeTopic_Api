using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Industry
{
    public class UpdateIndustryDto : BaseDto, IIndustryDto
    {
        public string InternalCode { get; set; }
        public string? Name { get; set; }
        public int? FacultyId { get; set; }
    }
}
