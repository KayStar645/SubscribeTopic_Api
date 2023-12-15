using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Features.Base.Requests.Queries;
using FluentValidation;

namespace Core.Application.Features.Duties.Requests.Queries
{
    public class ListFacultyDutyRequest : ListBaseRequest<FacultyDutyDto>
    {
        public bool? isGetFaculty { get; set; }

        public bool? isGetDepartment { get; set; }
    }

    public class ListFacultyDutyValidator : AbstractValidator<ListFacultyDutyRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListFacultyDutyValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Include(new ListBaseRequestValidator<FacultyDutyDto>());
        }
    }
}
