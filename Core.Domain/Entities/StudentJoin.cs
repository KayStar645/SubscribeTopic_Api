using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class StudentJoin : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public double? Score { get; set; }

        #endregion


        #region FOREIGN KEY

        // Sinh viên nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        // Đợt nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? RegistrationPeriodId { get; set; }
        [ForeignKey("RegistrationPeriodId")]
        public RegistrationPeriod? RegistrationPeriod { get; set; }

        // Nhóm nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        #endregion


        #region ICOLECTION

        public ICollection<Invitation> Invitations = new HashSet<Invitation>();

        // Danh sách điểm số của sinh viên
        public ICollection<Point> Points = new HashSet<Point>();

        #endregion


        #region FUNCTION

        #endregion
    }
}
