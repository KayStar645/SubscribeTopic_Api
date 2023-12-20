﻿using Core.Application.DTOs.Commissioner;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Council
{
    public class CouncilDto : BaseDto
    {
        public string? InternalCode { get; set; }

        public string? Name { get; set; }

        public DateTime? ProtectionDay { get; set; }

        public string? Location { get; set; }

        public List<CommissionerDto>? Commissioners { get; set; }
    }
}
