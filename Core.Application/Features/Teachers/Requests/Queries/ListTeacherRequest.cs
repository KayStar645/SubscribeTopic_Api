using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;
using FacultyEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.Features.Teachers.Requests.Queries
{
    public class ListTeacherRequest : ListBaseRequest<TeacherDto>
    {
        public bool? isGetDepartment { get; set; }
        
        public string? type { get; set; }

        public int? facultyId { get; set; }

        public int? departmentId { get; set; }
    }

    public class ListTeacherDtoValidator : AbstractValidator<ListTeacherRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListTeacherDtoValidator(IUnitOfWork unitOfWork, int? departmentId)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<TeacherDto>());

            if (departmentId != null)
            {
                RuleFor(x => x.departmentId)
                .MustAsync(async (departmentId, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentEntity>()
                        .FirstOrDefaultAsync(x => x.Id == departmentId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("departmentId"));
            }
            else
            {
                RuleFor(x => x.facultyId)
                .MustAsync(async (facultyId, token) =>
                {
                    var exists = await _unitOfWork.Repository<FacultyEntity>()
                        .FirstOrDefaultAsync(x => x.Id == facultyId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTransform.MustIn("facultyId"));
            }
        }
    }
}
