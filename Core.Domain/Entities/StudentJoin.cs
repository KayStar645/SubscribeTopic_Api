using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class StudentJoin : BaseAuditableEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student? Student { get; set;}


        [Sieve(CanFilter = true, CanSort = true)]
        public int? RegistrationPeriodId { get; set; }
        [ForeignKey("RegistrationPeriodId")]
        public RegistrationPeriod? RegistrationPeriod { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public double? Score { get; set; }
    }
}
