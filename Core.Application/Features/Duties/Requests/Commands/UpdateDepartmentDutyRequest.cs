using Core.Application.DTOs.Duty;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Duties.Requests.Commands
{
    public class UpdateDepartmentDutyRequest : IRequest<Result<DepartmentDutyDto>>
    {
        public UpdateDutyDto? updateDutyDto { get; set; }
    }
}
