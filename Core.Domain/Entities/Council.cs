using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Council : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES
        [Sieve(CanFilter = true, CanSort = true)]
        public string? InternalCode { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }   

        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "date")]
        public DateTime? ProtectionDay { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Location { get; set; }
        #endregion


        #region FOREIGN KEY
        // Hội đồng của khoa nào
        public int? FacultyId { get; set; }

        [ForeignKey("FacultyId")]
        public Faculties? Faculty { get; set; }
        #endregion


        #region ICOLECTION
        // Thành viên trong hội đồng
        public ICollection<Commissioner> Commissioners = new HashSet<Commissioner>();

        #endregion


        #region FUNCTION

        #endregion
    }
}
