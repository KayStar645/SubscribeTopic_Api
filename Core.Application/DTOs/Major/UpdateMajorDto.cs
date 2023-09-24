using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Major
{
    public class UpdateMajorDto : BaseDto, IMajorDto
    {
        public string InternalCode { get; set; }
        public string? Name { get; set; }
        public int? IndustryId { get; set; }
    }
}
