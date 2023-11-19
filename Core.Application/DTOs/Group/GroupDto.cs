using Core.Application.DTOs.StudentJoin;

namespace Core.Application.DTOs.Group
{
    public class GroupDto
    {
        public string? Name { get; set; }

        public int? CountMember { get; set; }

        public int? LeaderId { get; set; }

        public StudentJoinDto? Leader { get; set; }

        public List<StudentJoinDto>? Members { get; set; }
    }
}
