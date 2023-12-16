using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class ReportSchedule : BaseAuditableEntity
    {
        #region CONST
        [NotMapped]
        public const string TYPE_WEEKLY = "W"; // Báo cáo hằng tuần (Gặp mặt GVHD)
        [NotMapped]
        public const string TYPE_REVIEW = "R"; // Lịch phản biện (Có GVPB)
        #endregion


        #region PROPERTIES
        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "datetime")]
        public DateTime? TimeStart { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "datetime")]
        public DateTime? TimeEnd { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Location { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Type { get; set; }

        #endregion


        #region FOREIGN KEY
        // Lịch của đề tài nào
        public int? ThesisId { get; set; }

        [ForeignKey("ThesisId")]
        public Thesis? Thesis { get; set; }

        // Ai là người lên lịch
        public int? TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher? Teacher { get; set; }
        #endregion


        #region ICOLECTION

        #endregion


        #region FUNCTION
        public static string[] GetType()
        {
            return new string[]
            {
                TYPE_WEEKLY,
                TYPE_REVIEW
            };
        }
        #endregion
    }
}
