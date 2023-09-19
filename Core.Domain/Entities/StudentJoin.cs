using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class StudentJoin : BaseAuditableEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public int? StudentId { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]

        [ForeignKey("StudentId")]
        public Student? Student { get; set;}

        [Sieve(CanFilter = true, CanSort = true)]
        public int? Phase { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? Semester { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Year { get; set; }

        [ForeignKey("PeriodId")]
        public RegistrationPeriod? Period { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public double? Score { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public bool? isLeader { get; set; }
    }
}
