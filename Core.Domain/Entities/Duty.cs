using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Duty : BaseAuditableEntity
    {
        #region CONST
        // Loại nhiệm vụ
        [NotMapped]
        public const string TYPE_FACULTY = "F";
        [NotMapped]
        public const string TYPE_DEPARTMENT = "D";
        #endregion


        #region PROPERTIES
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }

        public string? Content { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? NumberOfThesis { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "datetime")]
        public DateTime? TimeEnd { get; set; }

        public string? Files { get; set; }

        public string? Type { get; set; }
        #endregion


        #region FOREIGN KEY
        // Nhiệm vụ của khoa nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculties? Faculty { get; set; }

        // Giao cho bộ môn nào/Nhiệm vụ của bộ môn nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        // Giao cho giảng viên nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? TeacherId { get; set; }
        [ForeignKey("DepartmentId")]
        public Teacher? Teacher { get; set; }

        // Nhiệm vụ này của đợt nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? PeriodId { get; set; }
        [ForeignKey("PeriodId")]
        public RegistrationPeriod? RegistrationPeriod { get; set; }

        // Nhiệm vụ này của nhiệm vụ nào (Nếu là nhiệm vụ khoa)
        [Sieve(CanFilter = true, CanSort = true)]
        public int? DutyId { get; set; }
        [ForeignKey("DutyId")]
        public Duty? ForDuty { get; set; }

        #endregion


        #region ICOLECTION
        // Danh sách đề tài cho nhiệm vụ này
        public ICollection<Thesis> Thesiss { get; set; } = new HashSet<Thesis>();
        #endregion


        #region FUNCTION
        public static string[] GetType()
        {
            return new string[]
            {
                TYPE_FACULTY,
                TYPE_DEPARTMENT
            };
        }
        #endregion
    }
}
