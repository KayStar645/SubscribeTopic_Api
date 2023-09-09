using Core.Domain.Common.Interfaces;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
    {

        [Sieve(CanFilter = false, CanSort = true)]
        [Column(TypeName = "datetime")]
        public DateTime? DateCreated { get; set; }

        public string? CreatedBy { get; set; }


        [Sieve(CanFilter = false, CanSort = true)]
        [Column(TypeName = "datetime")]
        public DateTime? LastModifiedDate { get; set; }

        public string? LastModifiedBy { get; set; }

        public bool? IsDeleted { get; set; } = false;
    }
}
