using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Exchanges;
using Core.Application.Features.Exchanges.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Exchanges.Handlers.Queries
{
    public class ListExchangeQueryHandler : IRequestHandler<ListExchangeRequest, PaginatedResult<List<ExchangeDto>>>
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListExchangeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<ExchangeDto>>> Handle(ListExchangeRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListExchangeDtoValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<ExchangeDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            // Start Query
            var query = _unitOfWork.Repository<Exchange>().GetAllInclude();

            query = query.Where(x => x.JobId == request.jobId);

            query = query.Include(x => x.Teacher);
            query = query.Include(x => x.Student);
            // End Query

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var groups = await query.ToListAsync();

            var mapGroups = _mapper.Map<List<ExchangeDto>>(groups);
            return PaginatedResult<List<ExchangeDto>>.Success(
                mapGroups, totalCount, request.page,
                request.pageSize);
        }
    }
}
