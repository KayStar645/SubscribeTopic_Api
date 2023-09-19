﻿using Core.Application.DTOs.Faculty;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.RegistrationPeriod
{
    public class RegistrationPeriodDto : BaseDto
    {
        public int? Phase { get; set; }
        public string? Semester { get; set; }
        public string? Year { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public int? FacultyId { get; set; }
        public FacultyDto? Faculty { get; set; }
    }
}
