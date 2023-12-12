using Core.Application.DTOs.Duty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Duties.Requests.Commands
{
    public class UpdateDutyRequest : IRequest<Result<DutyDto>>
    {
        public UpdateDutyDto? updateDutyDto { get; set; }
    }
}
