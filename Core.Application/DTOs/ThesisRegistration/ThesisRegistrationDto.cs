using Core.Application.DTOs.Group;
using Core.Application.DTOs.Thesis;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.ThesisRegistration
{
    public class ThesisRegistrationDto : BaseDto
    {
        public DateTime? DateCreated { get; set; }

        public int? GroupId { get; set; }

        public GroupDto? Group { get; set; }

        public int? ThesisId { get; set; }

        public ThesisDto? Thesis { get; set; }
    }
}
