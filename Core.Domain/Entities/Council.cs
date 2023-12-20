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
        [Column(TypeName = "datetime")]
        public DateTime? ProtectionDay { get; set; }

        public string? Location { get; set; }
        #endregion


        #region FOREIGN KEY
        // Thành viên trong hội đồng
        public ICollection<Commissioner> Commissioners = new HashSet<Commissioner>();

        #endregion


        #region ICOLECTION

        #endregion


        #region FUNCTION

        #endregion
    }
}
