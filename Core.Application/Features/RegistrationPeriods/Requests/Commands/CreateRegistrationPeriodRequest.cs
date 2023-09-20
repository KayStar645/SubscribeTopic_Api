using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.RegistrationPeriods.Requests.Commands
{
    public class CreateRegistrationPeriodRequest : IRequest<Result<RegistrationPeriodDto>>
    {
        public CreateRegistrationPeriodDto? CreateRegistrationPeriodDto { get; set; }
    }
}
