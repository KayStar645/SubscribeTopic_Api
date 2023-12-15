using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Duties.Requests.Queries
{
    public class DetailDepartmentDutyRequest : DetailBaseRequest, IRequest<Result<DepartmentDutyDto>>
    {
        public bool? isGetDepartment { get; set; }

        public bool? isGetTeacher { get; set; }

        public bool? isGetForDuty { get; set; }
    }
}
