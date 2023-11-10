﻿using Core.Domain.Common;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities
{
    public class Thesis : BaseAuditableEntity
    {
        #region CONST 

        #region CONST TYPE
        // Loại đề tài: Đề tài sv đề xuất hoặc đề tài giảng viên ra
        [NotMapped]
        public const string TYPE_LECTURER_OUT = "LO";
        [NotMapped]
        public const string TYPE_STUDENT_PROPOSAL = "SP";
        #endregion

        #region CONST STATUS
        // Trạng thái đề tài: (Nháp, yêu cầu duyệt, yêu cầu chỉnh sửa, Duyệt, Hủy) 
        // Giảng viên ra đề
        [NotMapped]
        public const string STATUS_DRAFT = "D";
        [NotMapped]
        public const string STATUS_APPROVE_REQUEST = "AR";
        [NotMapped]
        public const string STATUS_APPROVED = "A";
        [NotMapped]
        public const string STATUS_CANCEL = "C";

        // Sinh viên đề xuất

        #endregion

        #endregion


        #region PROPERTIES
        [Sieve(CanFilter = true, CanSort = true)]
        public string? InternalCode { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public string? Name { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public string? Summary { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public int? MinQuantity { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public int? MaxQuantity { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public string? Status { get; set; }


        [Sieve(CanFilter = true, CanSort = true)]
        public string? Type { get; set; }


        // Giảng viên ra đề
        [Sieve(CanFilter = true, CanSort = true)]
        public int? LecturerThesisId { get; set; }
        [ForeignKey(nameof(LecturerThesisId))]
        public Teacher? LecturerThesis { get; set; }


        // Sinh viên đề xuất
        [Sieve(CanFilter = true, CanSort = true)]
        public int? ProposedStudentId { get; set; }
        [ForeignKey(nameof(ProposedStudentId))]
        public Student? ProposedStudent { get; set; }


        // Nhóm đăng ký: chỉ được 1 nhóm (Nếu đề tài đề xuất thì auto gán nhóm)


        // Duyệt đề tài: Auto trưởng bộ môn tại thời điểm đó (DB là nhiều nhưng validator là 1)
        //* Duyệt đề tài
        // + Nếu giảng viên ra => auto trưởng bộ môn duyệt
        // + Nếu sinh viên đề xuất thì phải được giảng viên hướng dẫn duyệt và chỉnh sửa xong
        // mới chuyển qua trưởng bộ môn duyệt
        //*/


        #endregion

        // Giảng viên hướng dẫn
        [NotMapped]
        public ICollection<ThesisInstruction>? ThesisInstructions { get; set; } = new HashSet<ThesisInstruction>();

        // Giảng viên phản biện
        [NotMapped]
        public ICollection<ThesisReview>? ThesisReviews { get; set; } = new HashSet<ThesisReview>();

        // Chuyên ngành
        [NotMapped]
        public ICollection<ThesisMajor> ThesisMajors { get; set; } = new HashSet<ThesisMajor>();


        #region FUNCTION
        public static string[] GetType()
        {
            return new string[]
            {
                TYPE_LECTURER_OUT,
                TYPE_STUDENT_PROPOSAL
            };
        }

        public static string[] GetSatus()
        {
            return new string[]
            {
                STATUS_DRAFT,
                STATUS_APPROVE_REQUEST,
                STATUS_APPROVED,
                STATUS_CANCEL
            };
        }


        #endregion

    }
}
