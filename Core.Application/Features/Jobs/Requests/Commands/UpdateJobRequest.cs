using Core.Application.DTOs.Job;
using Core.Application.Responses;
using MediatR;

namespace Core.Application.Features.Jobs.Requests.Commands
{
    public class UpdateJobRequest : IRequest<Result<JobDto>>
    {
        public UpdateJobDto updateJobDto { get; set; }
    }
}
