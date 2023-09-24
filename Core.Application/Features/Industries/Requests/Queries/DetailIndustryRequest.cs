using Core.Application.DTOs.Industry;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Industries.Requests.Queries
{
    public class DetailIndustryRequest : DetailBaseRequest, IRequest<Result<IndustryDto>>
    {
        public bool isGetFaculty { get; set; }
    }
}
