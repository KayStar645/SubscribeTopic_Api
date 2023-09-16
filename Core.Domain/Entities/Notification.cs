using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Notification : BaseAuditableEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }
        public string? Describe {  get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public string? Images { get; set; }

        public int? FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculty? Faculty { get; set; }
    }
}
