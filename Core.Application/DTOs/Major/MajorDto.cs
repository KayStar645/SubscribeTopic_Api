using Core.Application.DTOs.Industry;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Major
{
    public class MajorDto : BaseDto
    {
        public string? InternalCode { get; set; }
        public string? Name { get; set; }
        public int? IndustryId { get; set; }
        public IndustryDto? Industry { get; set; }
    }
}
