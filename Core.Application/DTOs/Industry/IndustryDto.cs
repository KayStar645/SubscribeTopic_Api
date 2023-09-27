using Core.Application.DTOs.Faculty;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Industry
{
    public class IndustryDto : BaseDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public int? FacultyId { get; set; }
        public FacultyDto? Faculty { get; set; }
    }
}
