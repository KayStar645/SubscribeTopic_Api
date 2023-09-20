using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.RegistrationPeriods.Requests.Commands
{
    public class UpdateRegistrationPeriodRequest : IRequest<Result<RegistrationPeriodDto>>
    {
        public UpdateRegistrationPeriodDto? UpdateRegistrationPeriodDto { get; set; }
    }
}
