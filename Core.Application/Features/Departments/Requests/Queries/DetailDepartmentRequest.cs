using Core.Application.DTOs.Department;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Departments.Requests.Queries
{
    public class DetailDepartmentRequest : DetailBaseRequest, IRequest<Result<DepartmentDto>>
    {
        public bool IsGetFaculty { get; set; }
        public bool IsGetHeadDepartment { get; set; }
    }
}
