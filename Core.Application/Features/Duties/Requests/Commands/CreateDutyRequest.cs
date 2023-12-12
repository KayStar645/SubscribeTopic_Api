using Core.Application.DTOs.Duty;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Duties.Requests.Commands
{
    public class CreateDutyRequest : IRequest<Result<DutyDto>>
    {
        public CreateDutyDto? createDutyDto { get; set; }
    }
}
