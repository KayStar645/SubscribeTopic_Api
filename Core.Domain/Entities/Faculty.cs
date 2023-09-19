using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Faculty : BaseAuditableEntity
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

        // Trưởng khoa
        public int? Dean_TeacherId { get; set; }
        public Teacher? Dean_Teacher { get; set; }

        // Danh sách bộ môn và chuyên ngành của khoa
        [NotMapped]
        public ICollection<Department> Departments { get; } = new List<Department>();
        [NotMapped]
        public ICollection<Major> Majors { get; } = new HashSet<Major>();
        
        [NotMapped]
        public ICollection<Notification> Notifications { get; } = new HashSet<Notification>();

        [NotMapped]
        public ICollection<RegistrationPeriod> RegistrationPeriods { get; } = new HashSet<RegistrationPeriod>();
    }
}
