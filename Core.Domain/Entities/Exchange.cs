using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Exchange : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Content { get; set; }
        #endregion


        #region FOREIGN KEY
        // Trao đổi của công việc nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? JobId { get; set; }
        [ForeignKey("JobId")]
        public Job? Job { get; set; }

        // Sinh viên trao đổi
        [Sieve(CanFilter = true, CanSort = true)]
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        // Giảng viên trao đổi
        [Sieve(CanFilter = true, CanSort = true)]
        public int? TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher? Teacher { get; set; }
        #endregion


        #region ICOLECTION

        #endregion


        #region FUNCTION

        #endregion
    }
}
