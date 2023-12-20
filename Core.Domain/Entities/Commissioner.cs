using Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Commissioner : BaseAuditableEntity
    {
        #region CONST
        [NotMapped]
        public const string POSITION_MEMBER = "M"; // Ủy viên hội đồng (Thành viên)
        [NotMapped]
        public const string POSITION_CHAIRPERSON = "C"; // Chủ tịch
        [NotMapped]
        public const string POSITION_SECRETARY = "S"; // Thư kí


        #endregion
        public string? Position { get; set; }

        #region PROPERTIES

        #endregion


        #region FOREIGN KEY
        // Giáo viên nào
        public int? TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher? Teacher { get; set; }

        // Hội đồng nào
        public int? CouncilId { get; set; }

        [ForeignKey("CouncilId")]
        public Council? Council { get; set; }
        #endregion


        #region ICOLECTION

        #endregion


        #region FUNCTION
        public static string[] GetPosition()
        {
            return new string[]
            {
                POSITION_MEMBER,
                POSITION_CHAIRPERSON,
                POSITION_SECRETARY
            };
        }
        #endregion
    }
}
