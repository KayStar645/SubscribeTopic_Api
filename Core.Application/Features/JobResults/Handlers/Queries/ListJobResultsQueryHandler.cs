using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.JobResults;
using Core.Application.Features.JobResults.Requests.Queries;
using Core.Application.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;
using JobResultsEntity = Core.Domain.Entities.JobResults;

namespace Core.Application.Features.JobResultsResults.Handlers.Queries
{
    public class ListJobResultsQueryHandler : IRequestHandler<ListJobResultsRequest, PaginatedResult<List<JobResultsDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public ListJobResultsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<JobResultsDto>>> Handle(ListJobResultsRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListJobResultsDtoValidator(_unitOfWork, request.jobId, request.studentId);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<JobResultsDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<JobResultsEntity>().GetAllInclude();

            query = query.Where(x => x.JobId == request.jobId);

            if(request.studentId != null)
            {
                query = query.Where(x => x.StudentId == request.studentId);
            }


            if (request.isAllDetail || request.isGetForJob == true)
            {
                query = _unitOfWork.Repository<JobResultsEntity>().AddInclude(query, x => x.ForJob);
            }
            if (request.isAllDetail || request.isGetStudentBy == true)
            {
                query = _unitOfWork.Repository<JobResultsEntity>().AddInclude(query, x => x.StudentBy);
            }

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var JobResultss = await query.ToListAsync();

            var mapJobResultss = _mapper.Map<List<JobResultsDto>>(JobResultss);

            return PaginatedResult<List<JobResultsDto>>.Success(
                mapJobResultss, totalCount, request.page,
                request.pageSize);
        }
    }
}
