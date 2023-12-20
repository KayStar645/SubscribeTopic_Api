using Core.Application.DTOs.Commissioner;

namespace Core.Application.DTOs.Council
{
    public class CreateCouncilDto : ICouncilDto
    {
        public string? InternalCode { get; set; }

        public string? Name { get; set; }

        public DateTime? ProtectionDay { get; set; }

        public string? Location { get; set; }

        public List<CommissionerDto>? Commissioners { get; set; }
    }
}
