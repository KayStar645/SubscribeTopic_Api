using Core.Application.DTOs.DepartmentDuty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.DepartmentDuties.Requests.Commands
{
    public class CreateDepartmentDutyRequest : IRequest<Result<DepartmentDutyDto>>
    {
        public CreateDepartmentDutyDto? CreateDepartmentDutyDto { get; set; }
    }
}
