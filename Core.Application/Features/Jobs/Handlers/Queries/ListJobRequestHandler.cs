using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Job;
using Core.Application.Features.Jobs.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Jobs.Handlers.Queries
{
    public class ListJobRequestHandler : IRequestHandler<ListJobRequest, PaginatedResult<List<JobDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public ListJobRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<JobDto>>> Handle(ListJobRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListJobDtoValidator(_unitOfWork, request.thesisId);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<JobDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Job>().GetAllInclude();

            query = query.Where(x => x.ThesisId == request.thesisId);


            if (request.isAllDetail || request.isGetTeacher)
            {
                query = _unitOfWork.Repository<Job>().AddInclude(query, x => x.TeacherBy);
            }
            if (request.isAllDetail || request.isGetThesis)
            {
                query = _unitOfWork.Repository<Job>().AddInclude(query, x => x.ForThesis);
            }

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var Jobs = await query.ToListAsync();

            var mapJobs = _mapper.Map<List<JobDto>>(Jobs);

            return PaginatedResult<List<JobDto>>.Success(
                mapJobs, totalCount, request.page,
                request.pageSize);
        }
    }
}
