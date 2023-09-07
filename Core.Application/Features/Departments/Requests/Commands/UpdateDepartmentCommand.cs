using Core.Application.DTOs.Department;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Departments.Requests.Commands
{
    public class UpdateDepartmentCommand : IRequest<Result<DepartmentDto>>
    {
        public UpdateDepartmentDto? UpdateDepartmentDto { get; set; }
    }
}
