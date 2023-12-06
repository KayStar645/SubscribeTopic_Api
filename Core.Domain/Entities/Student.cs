using Core.Domain.Common;
using Core.Domain.Entities.Identity;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Student : BaseAuditableEntity
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
        public DateTime? DateOfBirth { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Gender { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Class { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? PhoneNumber { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Email { get; set; }

        #endregion


        #region FOREIGN KEY

        // Sinh viên thuộc chuyên ngành nào
        public int? MajorId { get; set; }

        [ForeignKey("MajorId")]
        public Major? Major { get; set; }

        // User nào
        public int? UserId { get; set; }
        public User? User { get; set; }

        #endregion


        #region ICOLECTION

        // Các đợt mà sinh viên tham gia
        public ICollection<StudentJoin> StudentJoins = new HashSet<StudentJoin>();

        // Các kết quả công việc mà sinh viên nộp
        public ICollection<JobResults> JobResults = new HashSet<JobResults>();

        #endregion


        #region FUNCTION

        #endregion

    }
}
