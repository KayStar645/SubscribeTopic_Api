using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Features.Base.Requests.Queries;
using FluentValidation;

namespace Core.Application.Features.Duties.Requests.Queries
{
    public class ListDepartmentDutyRequest : ListBaseRequest<DepartmentDutyDto>
    {
        public bool? isGetDepartment { get; set; }

        public bool? isGetTeacher { get; set; }

        public bool? isGetForDuty { get; set; }
    }

    public class ListDepartmentDutyValidator : AbstractValidator<ListDepartmentDutyRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListDepartmentDutyValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<DepartmentDutyDto>());
        }
    }
}
