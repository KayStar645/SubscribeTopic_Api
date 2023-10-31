using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Notification : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }
        public string? Describe { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public string? Images { get; set; }

        #endregion


        #region FOREIGN KEY

        // Thông báo của khoa nào. Không có => toàn trường
        public int? FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculties? Faculty { get; set; }

        #endregion


        #region ICOLECTION

        #endregion


        #region FUNCTION

        #endregion
    }
}
