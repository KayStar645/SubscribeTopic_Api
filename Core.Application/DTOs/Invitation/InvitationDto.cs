using Core.Application.DTOs.Group;
using Core.Application.DTOs.StudentJoin;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Invitation
{
    public class InvitationDto : BaseDto
    {
        public string? Message { get; set; }

        public DateTime? TimeSent { get; set; }

        public string? Status { get; set; }

        public int? GroupId { get; set; }

        public GroupDto? Group { get; set; }

        public int? StudentJoinId { get; set; }

        public StudentJoinDto? StudentJoinDto { get; set; }
    }
}
