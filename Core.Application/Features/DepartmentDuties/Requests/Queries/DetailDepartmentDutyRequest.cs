using Core.Application.DTOs.DepartmentDuty;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.DepartmentDuties.Requests.Queries
{
    public class DetailDepartmentDutyRequest : DetailBaseRequest, IRequest<Result<DepartmentDutyDto>>
    {
        public bool isGetDepartment { get; set; }
        public bool isGetTeacher { get; set; }
    }
}
