using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Group : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public int? CountMember { get; set; }

        #endregion


        #region FOREIGN KEY

        // Trưởng nhóm
        [Sieve(CanFilter = true, CanSort = true)]
        public int? LeaderId { get; set; }
        [ForeignKey("LeaderId")]
        public StudentJoin? Leader { get; set; }

        // Đăng ký đề tài nào
        public ThesisRegistration? ThesisRegistration { get; set; }

        #endregion


        #region ICOLECTION

        [NotMapped]
        public ICollection<StudentJoin> Members = new HashSet<StudentJoin>();

        [NotMapped]
        public ICollection<Invitation> Invitations = new HashSet<Invitation>();

        #endregion


        #region FUNCTION

        #endregion
    }
}
