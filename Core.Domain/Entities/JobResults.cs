using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class JobResults : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES
        public string? Files { get; set; }

        #endregion


        #region FOREIGN KEY
        // Ai là người nộp
        [Sieve(CanFilter = true, CanSort = true)]
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        [Sieve(CanFilter = true, CanSort = true)]
        public Student? StudentBy { get; set; }

        // Nộp kết quả cho công việc nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? JobId { get; set; }
        [ForeignKey("JobId")]
        [Sieve(CanFilter = true, CanSort = true)]
        public Job? ForJob { get; set; }
        #endregion


        #region ICOLECTION

        #endregion


        #region FUNCTION

        #endregion
    }
}
