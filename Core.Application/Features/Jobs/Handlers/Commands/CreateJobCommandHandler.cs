using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Job;
using Core.Application.DTOs.Job.Validators;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.Jobs.Events;
using Core.Application.Features.Jobs.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Jobs.Handlers.Commands
{
    public class CreateJobCommandHandler : IRequestHandler<CreateJobRequest, Result<JobDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateJobCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<JobDto>> Handle(CreateJobRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateJobDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createJobDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<JobDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var job = _mapper.Map<Job>(request.createJobDto);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateJobUpdateJobEvent(job, httpContextAccessor, unitOfWork));

                    }
                });

                var newJob = await _unitOfWork.Repository<Job>().AddAsync(job);
                await _unitOfWork.Save(cancellationToken);

                var jobDto = _mapper.Map<JobDto>(newJob);

                return Result<JobDto>.Success(jobDto, (int)HttpStatusCode.Created);
            }
            catch (NotFoundException ex)
            {
                return Result<JobDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<JobDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<JobDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<JobDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}