using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Department;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using FacultyEntity = Core.Domain.Entities.Faculties;

namespace Core.Application.Features.Departments.Requests.Queries
{
    public class ListDepartmentRequest : ListBaseRequest<DepartmentDto>
    {
        public bool isGetFaculty { get; set; }

        public bool isGetHeadDepartment { get; set; }

        public int facultyId { get; set; }
    }

    public class DepartmentDtoValidator : AbstractValidator<ListDepartmentRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<DepartmentDto>());

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
