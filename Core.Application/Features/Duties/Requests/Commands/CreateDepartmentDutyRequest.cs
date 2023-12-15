using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Duties.Requests.Commands
{
    public class CreateDepartmentDutyRequest : IRequest<Result<DepartmentDutyDto>>
    {
        public CreateDepartmentDutyDto? createDepartmentDutyDto { get; set; }
    }
}
