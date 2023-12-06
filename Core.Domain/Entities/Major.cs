using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Major : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public string? InternalCode { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]

        #endregion


        #region FOREIGN KEY

        public int? IndustryId { get; set; }

        [ForeignKey("IndustryId")]
        public Industry? Industry { get; set; }


        #endregion


        #region ICOLECTION

        // Danh sách sinh viên trong chuyên ngành
        public ICollection<Student> Students { get; } = new HashSet<Student>();

        #endregion


        #region FUNCTION

        #endregion

    }
}
