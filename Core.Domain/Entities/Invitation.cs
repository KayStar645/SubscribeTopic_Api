using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Invitation : BaseAuditableEntity
    {
        #region CONST

        [NotMapped]
        public const string STATUS_SENT = "S";
        [NotMapped]
        public const string STATUS_ACCEPT = "A";

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Message { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "datetime")]
        public DateTime? TimeSent { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public string? Status { get; set; }

        #endregion


        #region FOREIGN KEY

        // Nhóm nào mời
        [Sieve(CanFilter = true, CanSort = true)]
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Group? Group { get; set; }

        // Mời sinh viên nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? StudentJoinId { get; set; }
        [ForeignKey("StudentJoinId")]
        public StudentJoin? StudentJoin { get; set; }

        #endregion


        #region ICOLECTION

        #endregion


        #region FUNCTION

        public static string[] GetSatus()
        {
            return new string[]
            {
                STATUS_SENT,
                STATUS_ACCEPT
            };
        }

        #endregion
    }
}
