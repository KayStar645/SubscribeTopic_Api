using Core.Application.DTOs.DepartmentDuty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.DepartmentDuties.Requests.Commands
{
    public class UpdateDepartmentDutyRequest : IRequest<Result<DepartmentDutyDto>>
    {
        public UpdateDepartmentDutyDto? UpdateDepartmentDutyDto { get; set; }
    }
}
