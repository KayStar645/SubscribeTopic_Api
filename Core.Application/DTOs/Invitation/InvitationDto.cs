using Core.Application.DTOs.Group;
using Core.Application.DTOs.StudentJoin;

namespace Core.Application.DTOs.Invitation
{
    public class InvitationDto
    {
        public string? Message { get; set; }

        public DateTime? TimeSent { get; set; }

        public bool? Status { get; set; }

        public int? GroupId { get; set; }

        public GroupDto? Group { get; set; }

        public int? StudentJoinId { get; set; }

        public StudentJoinDto? StudentJoinDto { get; set; }
    }
}
