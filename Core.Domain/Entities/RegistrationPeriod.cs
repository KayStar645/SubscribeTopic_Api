using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class RegistrationPeriod : BaseAuditableEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Phase { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Semester { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Year { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "date")]
        public DateTime? TimeStart { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "date")]
        public DateTime? TimeEnd { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? FacultyId { get; set; }

        [ForeignKey("FacultyId")]
        public Faculty? Faculty { get; set; }

        [NotMapped]
        public ICollection<StudentJoin> StudentJoins = new HashSet<StudentJoin>();
    }
}
