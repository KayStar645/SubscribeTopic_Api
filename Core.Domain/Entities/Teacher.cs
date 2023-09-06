﻿using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Teacher : BaseAuditableEntity
    {

        [Sieve(CanFilter = true, CanSort = true)]
        public string InternalCode { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int DepartmentId { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Gender { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "datetime")]
        public DateTime DateOfBirth { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string PhoneNumber { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Email { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string AcademicTitle { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Degree { get; set; }

    }
}
