using Core.Application.DTOs.ThesisRegistration;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.ThesisRegistrations.Requests.Queries
{
    public class DetailThesisRegistrationRequest : DetailBaseRequest, IRequest<Result<ThesisRegistrationDto>>
    {
        public bool? isGetGroup {  get; set; }

        public bool? isGetThesis { get; set; }
    }
}
