using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.DepartmentDuty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;
using TeacherEntity = Core.Domain.Entities.Teacher;

namespace Core.Application.Features.DepartmentDuties.Requests.Queries
{
    public class ListDepartmentDutyRequest : ListBaseRequest<DepartmentDutyDto>
    {
        public bool? isGetDepartment { get; set; }
        public bool? isGetTeacher { get; set; }

        public int? departmentId { get; set; }
        public int? teacherId { get; set; }

    }

    public class ListDepartmentDutyDtoValidator : AbstractValidator<ListDepartmentDutyRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListDepartmentDutyDtoValidator(IUnitOfWork unitOfWork, int? teacherId)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<DepartmentDutyDto>());

            if (teacherId != null)
            {
                RuleFor(x => x.teacherId)
                    .MustAsync(async (teacherId, token) =>
                    {
                        var exists = await _unitOfWork.Repository<TeacherEntity>()
                            .FirstOrDefaultAsync(x => x.Id == teacherId);
                        return exists != null;
                    })
                    .WithMessage(id => ValidatorTransform.MustIn("teacherId"));
            }
            else
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
        }
    }
}
