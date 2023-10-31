﻿using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Faculties : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public string? InternalCode { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Address { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? PhoneNumber { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Email { get; set; }

        #endregion


        #region FOREIGN KEY

        // Trưởng khoa
        public int? Dean_TeacherId { get; set; }
        public Teacher? Dean_Teacher { get; set; }

        #endregion


        #region ICOLECTION

        // Bộ môn của khoa
        [NotMapped]
        public ICollection<Department>? Departments { get; } = new HashSet<Department>();

        // Ngành của khoa
        [NotMapped]
        public ICollection<Industry>? Industries { get; } = new HashSet<Industry>();

        // Thông báo của khoa
        [NotMapped]
        public ICollection<Notification>? Notifications { get; } = new HashSet<Notification>();

        // Đợt đăng ký của khoa
        [NotMapped]
        public ICollection<RegistrationPeriod>? RegistrationPeriods { get; } = new HashSet<RegistrationPeriod>();

        // Khoa giao nhiệm vụ
        [NotMapped]
        public ICollection<FacultyDuty>? FacultyDuties { get; } = new HashSet<FacultyDuty>();

        #endregion


        #region FUNCTION

        #endregion





    }
}
