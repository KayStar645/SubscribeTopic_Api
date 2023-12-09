using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Job;
using Core.Application.DTOs.JobResults;
using Core.Application.DTOs.JobResults.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.JobResults.Events;
using Core.Application.Features.JobResults.Requests.Commands;
using Core.Application.Features.Jobs.Events;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using JobResultsEntity = Core.Domain.Entities.JobResults;

namespace Core.Application.Features.JobResults.Handlers.Commands
{
    public class SubmitJobResultsCommandHandler : IRequestHandler<CreateJobResultsRequest, Result<JobResultsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public SubmitJobResultsCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<JobResultsDto>> Handle(CreateJobResultsRequest request, CancellationToken cancellationToken)
        {
            var validator = new SubmitJobResultsDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createJobResultsDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<JobResultsDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var jobResults = _mapper.Map<JobResultsEntity>(request.createJobResultsDto);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateJobResultsUpdateJobResultsEvent(jobResults, httpContextAccessor, unitOfWork));
                    }
                });

                if(jobResults.StudentId == null)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }

                // Nếu tồn tại thì cập nhật
                var jonRequestFind = await _unitOfWork.Repository<JobResultsEntity>().Query()
                                             .Where(x => x.StudentId == jobResults.StudentId && x.JobId == jobResults.JobId)
                                             .FirstOrDefaultAsync();
                JobResultsEntity newJobResults = null;
                if (jonRequestFind != null)
                {
                    jonRequestFind.Files = jobResults.Files;

                    newJobResults = await _unitOfWork.Repository<JobResultsEntity>().UpdateAsync(jonRequestFind);
                    await _unitOfWork.Save(cancellationToken);
                }
                else
                {
                    newJobResults = await _unitOfWork.Repository<JobResultsEntity>().AddAsync(jobResults);
                    await _unitOfWork.Save(cancellationToken);
                }

                var jobResultsDto = _mapper.Map<JobResultsDto>(newJobResults);

                return Result<JobResultsDto>.Success(jobResultsDto, (int)HttpStatusCode.Created);
            }
            catch (NotFoundException ex)
            {
                return Result<JobResultsDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<JobResultsDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<JobResultsDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<JobResultsDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
