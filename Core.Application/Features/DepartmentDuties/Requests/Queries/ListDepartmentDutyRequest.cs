using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.DepartmentDuty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Transform;
using FluentValidation;
using DepartmentEntity = Core.Domain.Entities.Department;

namespace Core.Application.Features.DepartmentDuties.Requests.Queries
{
    public class ListDepartmentDutyRequest : ListBaseRequest<DepartmentDutyDto>
    {
        public bool isGetDepartment { get; set; }

        public int departmentId { get; set; }
    }

    public class DepartmentDutyDtoValidator : AbstractValidator<ListDepartmentDutyRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentDutyDtoValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<DepartmentDutyDto>());

            RuleFor(x => x.departmentId)
                .MustAsync(async (departmentId, token) =>
                {
                    var exists = await _unitOfWork.Repository<DepartmentEntity>()
                        .FirstOrDefaultAsync(x => x.Id == departmentId);
                    return exists != null;
                })
                .WithMessage(id => ValidatorTranform.MustIn("departmentId"));
        }
    }
}
