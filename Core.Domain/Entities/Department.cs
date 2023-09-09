using Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Department : BaseAuditableEntity
    {
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

        [Sieve(CanFilter = true, CanSort = true)]
        public int? FacultyId { get; set; }


        [ForeignKey("FacultyId")]
        public Faculty? Faculty { get; set; }

        public IList<Teacher> Teachers { get; } = new List<Teacher>();
    }
}
