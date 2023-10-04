using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.FacultyDuty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;
using FacultyEntity = Core.Domain.Entities.Faculty;

namespace Core.Application.Features.FacultyDuties.Requests.Queries
{
    public class ListFacultyDutyRequest : ListBaseRequest<FacultyDutyDto>
    {
        public bool? isGetFaculty { get; set; }
        public bool? isGetDepartment { get; set; }
        public int? facultyId { get; set; }
        public int? departmentId { get; set; }
    }

    public class ListFacultyDutyDtoValidator : AbstractValidator<ListFacultyDutyRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListFacultyDutyDtoValidator(IUnitOfWork unitOfWork, int? departmentId)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<FacultyDutyDto>());

            if (departmentId != null)
            {
                RuleFor(x => x.departmentId)
                    .MustAsync(async (departmentId, token) =>
                    {
                        var exists = await _unitOfWork.Repository<DepartmentEntity>()
                            .FirstOrDefaultAsync(x => x.Id == departmentId);
                        return exists != null;
                    })
                    .WithMessage(id => ValidatorTranform.MustIn("departmentId"));
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
                    .WithMessage(id => ValidatorTranform.MustIn("facultyId"));
            }
        }
    }
}
