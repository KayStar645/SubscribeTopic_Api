using Core.Domain.Common;
using Core.Domain.Entities.Identity;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Teacher : BaseAuditableEntity
    {
        #region CONST

        // Type
        [NotMapped]
        public const string TYPE_TEACHER_MINISTRY = "M";
        [NotMapped]
        public const string TYPE_TEACHER_LECTURERS = "L";

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public string? InternalCode { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Gender { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? PhoneNumber { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Email { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? AcademicTitle { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Degree { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Type { get; set; }

        #endregion


        #region FOREIGN KEY

        // Thuộc bộ môn nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        // Là trưởng khoa của khoa nào
        public Faculty? Dean_Faculty { get; set; }

        // Là trưởng bộ môn của bộ môn nào nào
        public Department? HeadDepartment_Department { get; set; }

        // User nào
        public int? UserId { get; set; }
        public User? User { get; set; }

        #endregion


        #region ICOLECTION

        #endregion


        #region FUNCTION

        public static string[] GetType()
        {
            return new string[]
            {
                TYPE_TEACHER_MINISTRY,
                TYPE_TEACHER_LECTURERS
            };
        }

        #endregion
    }
}
