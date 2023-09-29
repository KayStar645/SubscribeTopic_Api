﻿using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class DepartmentDuty : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public string? InternalCode { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? NumberOfThesis { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? TimeStart { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? TimeEnd { get; set; }

        public string? Image { get; set; }
        public string? File { get; set; }

        #endregion


        #region FOREIGN KEY

        // Thuộc khoa nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        // Giao cho giảng viên nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? TeacherId { get; set; }
        [ForeignKey("DepartmentId")]
        public Teacher? Teacher { get; set; }

        #endregion


        #region ICOLECTION

        #endregion


        #region FUNCTION

        #endregion
    }
}
