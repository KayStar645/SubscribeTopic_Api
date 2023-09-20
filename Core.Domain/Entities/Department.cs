using Core.Domain.Common;
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

        // Trưởng bộ môn
        public int? HeadDepartment_TeacherId { get; set; }
        public Teacher? HeadDepartment_Teacher { get; set; }

        // Thuộc khoa nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculty? Faculty { get; set; }


        // Danh sách giảng viên trong bộ môn
        [NotMapped]
        public ICollection<Teacher> Teachers { get; } = new HashSet<Teacher>();
    }
}
