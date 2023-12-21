using Core.Application.DTOs.Commissioner;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Council
{
    public class UpdateCouncilDto : BaseDto, ICouncilDto
    {
        public string? InternalCode { get; set; }

        public string? Name { get; set; }

        public string? Location { get; set; }

        public List<CreateCommissionerDto>? Commissioners { get; set; }
    }
}
