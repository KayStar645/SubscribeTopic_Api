using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.RegistrationPeriods.Requests.Queries
{
    public class DetailRegistrationPeriodRequest : DetailBaseRequest, IRequest<Result<RegistrationPeriodDto>>
    {
        public bool isGetFaculty { get; set; }
    }
}
