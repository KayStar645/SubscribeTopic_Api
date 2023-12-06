using Core.Application.DTOs.Job;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Jobs.Requests.Queries
{
    public class DetailJobRequest : DetailBaseRequest, IRequest<Result<JobDto>>
    {
        public bool isGetTeacher { get; set; }
        public bool isGetThesis { get; set; }
    }
}
