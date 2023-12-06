using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Industry : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public string? InternalCode { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }

        #endregion


        #region FOREIGN KEY
        // Thuộc khoa nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculties? Faculty { get; set; }

        #endregion


        #region ICOLECTION

        // Dành sách chuyên ngành trong ngành
        public ICollection<Major>? Majors { get; set; } = new HashSet<Major>();

        #endregion


        #region FUNCTION

        #endregion

    }
}
