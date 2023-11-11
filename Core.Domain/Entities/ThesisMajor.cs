using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class ThesisMajor : BaseAuditableEntity
    {
        #region CONST
        #endregion

        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public int? ThesisId { get; set; }
        [ForeignKey(nameof(ThesisId))]
        public Thesis? Thesis { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public int? MajorId { get; set; }
        [ForeignKey(nameof(MajorId))]
        public Major? Major { get; set; }

        #endregion



        #region FUNCTION
        #endregion
    }
}
