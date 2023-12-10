using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Job : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES
        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }

        public string? Instructions { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        [Column(TypeName = "datetime")]
        public DateTime? Due { get; set; }

        public string? Files { get; set; }
        #endregion


        #region FOREIGN KEY
        // Giảng viên ra đề
        [Sieve(CanFilter = true, CanSort = true)]
        public int? TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        [Sieve(CanFilter = true, CanSort = true)]
        public Teacher? TeacherBy { get; set; }

        // Công việc của đề tài nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? ThesisId { get; set; }
        [ForeignKey("ThesisId")]
        [Sieve(CanFilter = true, CanSort = true)]
        public Thesis? ForThesis { get; set; }

        #endregion


        #region ICOLECTION
        // Các kết quả công việc của công việc này
        public ICollection<JobResults> JobResults = new HashSet<JobResults>();
        
        // Có những trao đổi trong công việc
        public ICollection<Exchange> Exchanges { get; set; } = new HashSet<Exchange>();
        #endregion


        #region FUNCTION

        #endregion
    }
}
