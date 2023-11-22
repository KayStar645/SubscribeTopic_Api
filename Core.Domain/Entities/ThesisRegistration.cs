using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class ThesisRegistration : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        #endregion


        #region FOREIGN KEY
        // Nhóm nào đăng ký
        [Sieve(CanFilter = true, CanSort = true)]
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Group? Group { get; set; }

        // Đăng ký đề tài nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? ThesisId { get; set; }
        [ForeignKey("ThesisId")]
        public Thesis? Thesis { get; set; }
        #endregion


        #region ICOLECTION

        #endregion


        #region FUNCTION

        #endregion
    }
}
