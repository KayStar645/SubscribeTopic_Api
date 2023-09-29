using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class FacultyDuty : BaseAuditableEntity
    {
        #region CONST

        #endregion


        #region PROPERTIES

        [Sieve(CanFilter = true, CanSort = true)]
        public string? InternalCode { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string? NumberOfThesis { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? TimeStart { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime? TimeEnd { get; set; }

        public string? Image { get; set; }
        public string? File { get; set; }

        #endregion


        #region FOREIGN KEY

        // Thuộc khoa nào
        [Sieve(CanFilter = true, CanSort = true)]
        public int? FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public Faculty? Faculty { get; set; }

        // Bộ môn nào được giao nhiệm vụ
        [Sieve(CanFilter = true, CanSort = true)]
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        #endregion


        #region ICOLECTION


        #endregion


        #region FUNCTION

        #endregion
    }
}
