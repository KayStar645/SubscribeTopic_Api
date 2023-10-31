using Core.Application.DTOs.Department;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Departments.Requests.Commands
{
    public class CreateDepartmentRequest : IRequest<Result<DepartmentDto>>
    {
        public CreateDepartmentDto? createDepartmentDto { get; set; }
    }
}
