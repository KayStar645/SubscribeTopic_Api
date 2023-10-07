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
        public StudentJoin? StudentJoin { get; set; }

        #endregion


        #region ICOLECTION

        [NotMapped]
        public ICollection<StudentJoin> StudentJoins = new HashSet<StudentJoin>();

        #endregion


        #region FUNCTION

        #endregion
    }
}
