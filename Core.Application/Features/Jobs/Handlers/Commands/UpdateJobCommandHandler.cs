using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Job;
using Core.Application.DTOs.Job.Validators;
using Core.Application.Features.Jobs.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Jobs.Handlers.Commands
{
    public class UpdateJobCommandHandler : IRequestHandler<UpdateJobRequest, Result<JobDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateJobCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<JobDto>> Handle(UpdateJobRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateJobDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.updateJobDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<JobDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findJob = await _unitOfWork.Repository<Job>().GetByIdAsync(request.updateJobDto.Id);

                if (findJob is null)
                {
                    return Result<JobDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.updateJobDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findJob.CopyPropertiesFrom(request.updateJobDto);

                var newJob = await _unitOfWork.Repository<Job>().UpdateAsync(findJob);
                await _unitOfWork.Save(cancellationToken);

                var jobDto = _mapper.Map<JobDto>(newJob);

                return Result<JobDto>.Success(jobDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<JobDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
