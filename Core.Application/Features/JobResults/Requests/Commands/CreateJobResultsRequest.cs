using Core.Application.DTOs.JobResults;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.JobResults.Requests.Commands
{
    public class CreateJobResultsRequest : IRequest<Result<JobResultsDto>>
    {
        public SubmitJobResultsDto createJobResultsDto { get; set; }
    }
}
