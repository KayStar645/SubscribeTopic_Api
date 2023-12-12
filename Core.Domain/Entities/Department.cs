using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Department : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

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

        #endregion


        #region FOREIGN KEY

        // Trưởng bộ môn
        public int? HeadDepartment_TeacherId { get; set; }
        public Teacher? HeadDepartment_Teacher { get; set; }

        // Thuộc khoa nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculties? Faculty { get; set; }

        #endregion


        #region ICOLECTION

        // Danh sách giảng viên trong bộ môn
        public ICollection<Teacher>? Teachers { get; } = new HashSet<Teacher>();

        // Danh sách nhiệm vụ khoa giao cho bộ môn/Danh sách nhiệm vụ bộ môn giao cho giảng viên
        public ICollection<Duty>? Duties { get; } = new HashSet<Duty>();


        #endregion


        #region FUNCTION

        #endregion
    }
}
