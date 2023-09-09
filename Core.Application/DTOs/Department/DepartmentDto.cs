﻿using Core.Application.DTOs.Faculty;
using KLTN.Core.Application.DTOs.Common;

namespace Core.Application.DTOs.Department
{
    public class DepartmentDto : BaseDto
    {
        public int? FacultyId { get; set; }
        public string InternalCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }


        public FacultyDto? Faculty { get; set; }
    }
}
