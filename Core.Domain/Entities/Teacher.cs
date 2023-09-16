using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Teacher : BaseAuditableEntity
    {
        [NotMapped]
        public const string TYPE_TEACHER_MINISTRY = "M";
        [NotMapped]
        public const string TYPE_TEACHER_LECTURERS = "L";


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


        [Sieve(CanFilter = true, CanSort = true)]
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        // Trưởng khoa
        public Faculty? Dean_Faculty { get; set; }

        // Trưởng bộ môn
        public Department? HeadDepartment_Department { get; set; }

        public static string[] GetType()
        {
            return new string[]
            {
                TYPE_TEACHER_MINISTRY,
                TYPE_TEACHER_LECTURERS
            };
        }

    }
}
