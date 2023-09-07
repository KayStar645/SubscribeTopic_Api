using Core.Domain.Common;
using Sieve.Attributes;

namespace Core.Domain.Entities
{
    public class Department : BaseAuditableEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public string InternalCode { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Address { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string PhoneNumber { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Email { get; set; }
    }
}
