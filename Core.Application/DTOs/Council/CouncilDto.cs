using Core.Application.DTOs.Commissioner;
using Core.Domain.Entities;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Council
{
    public class CouncilDto : BaseDto
    {
        public string? InternalCode { get; set; }

        public string? Name { get; set; }

        public string? Location { get; set; }

        public List<CommissionerDto>? Commissioners { get; set; }

        public int? FacultyId { get; set; }

        public Faculties? Faculty { get; set; }
    }
}
