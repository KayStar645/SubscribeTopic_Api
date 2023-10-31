using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class RegistrationPeriod : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public string? SchoolYear { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public string? Semester { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public int? Phase { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "datetime")]
        public DateTime? TimeStart { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "datetime")]
        public DateTime? TimeEnd { get; set; }

        #endregion


        #region FOREIGN KEY

        // Đợt này của khoa nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculty? Faculty { get; set; }

        #endregion


        #region ICOLECTION

        // Sinh viên tham gia đợt đăng ký này
        [NotMapped]
        public ICollection<StudentJoin> StudentJoins = new HashSet<StudentJoin>();

        #endregion


        #region FUNCTION

        #endregion        
    }
}
