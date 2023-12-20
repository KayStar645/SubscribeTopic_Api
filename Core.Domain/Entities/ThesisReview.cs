using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class ThesisReview : BaseAuditableEntity
    {
        #region CONST
        #endregion

        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public int? TeacherId { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public Teacher? Teacher { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public int? ThesisId { get; set; }
        [ForeignKey(nameof(ThesisId))]
        public Thesis? Thesis { get; set; }


        [DefaultValue(false)]
        public bool IsApprove { get; set; }

        #endregion



        #region FUNCTION
        #endregion
    }
}
