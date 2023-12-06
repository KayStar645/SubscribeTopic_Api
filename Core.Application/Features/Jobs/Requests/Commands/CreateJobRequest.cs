using Core.Application.DTOs.Job;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Jobs.Requests.Commands
{
    public class CreateJobRequest : IRequest<Result<JobDto>>
    {
        public CreateJobDto createJobDto { get; set; }
    }
}
