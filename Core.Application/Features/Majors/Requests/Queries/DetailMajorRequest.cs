using Core.Application.DTOs.Major;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Majors.Requests.Queries
{
    public class DetailMajorRequest : DetailBaseRequest, IRequest<Result<MajorDto>>
    {
        public bool isGetIndustry {  get; set; }
    }
}
