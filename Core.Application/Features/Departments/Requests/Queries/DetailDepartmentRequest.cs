using Core.Application.DTOs.Department;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Departments.Requests.Queries
{
    public class DetailDepartmentRequest : IRequest<Result<DepartmentDto>>
    {
        public int Id { get; set; }
    }
}
