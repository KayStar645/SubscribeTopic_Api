using Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Point : BaseAuditableEntity
    {
        #region CONST
        [NotMapped]
        public const string TYPE_INSTRUCTION = "I";
        [NotMapped]
        public const string TYPE_REVIEW = "R";
        [NotMapped]
        public const string TYPE_COUNCIL = "C";
        #endregion


        #region PROPERTIES
        public double? Scores { get; set; }
        public string? Type { get; set; }
        #endregion


        #region FOREIGN KEY
        // Điểm của sinh viên nào
        public int? StudentJoinId { get; set; }

        [ForeignKey("StudentJoinId")]
        public StudentJoin? StudentJoin { get; set; }

        // Giáo viên nào chấm điểm
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
