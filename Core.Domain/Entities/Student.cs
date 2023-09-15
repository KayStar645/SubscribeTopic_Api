using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Student : BaseAuditableEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public string? InternalCode { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? DateOfBirth { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Gender { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Class { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? PhoneNumber { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Email { get; set; }
        public int? MajorId { get; set; }

        [ForeignKey("MajorId")]
        public Major? Major { get; set; }
    }
}
