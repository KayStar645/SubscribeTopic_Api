using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Thesis
{
    public class ChangeStatusThesisDto : BaseDto
    {
        public string? Status { get; set; }

        public int? DutyId { get; set; }

    }
}
