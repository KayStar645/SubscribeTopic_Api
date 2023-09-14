using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Major : BaseAuditableEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public string? InternalCode { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int? FacultyId { get; set; }

        [ForeignKey("FacultyId")]
        public Faculty? Faculty { get; set; }
    }
}
