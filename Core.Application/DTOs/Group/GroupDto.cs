using Core.Application.DTOs.StudentJoin;

namespace Core.Application.DTOs.Group
{
    public class GroupDto
    {
        public string? name { get; set; }

        public int? countMember { get; set; }

        public int? leaderId { get; set; }

        public StudentJoinDto? leader { get; set; }

        public List<StudentJoinDto>? members { get; set; }
    }
}
