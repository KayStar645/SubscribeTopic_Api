using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Feedback : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        public string? Content { get; set; }

        #endregion


        #region FOREIGN KEY
        [Sieve(CanFilter = true, CanSort = false)]
        public int? CommenterId { get; set; }

        [ForeignKey("CommenterId")]
        public Teacher? Commenter { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
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
