using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.Features.RegistrationPeriods.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.RegistrationPeriods.Handlers.Queries
{
    public class ListRegistrationPeriodRequestHandler : IRequestHandler<ListRegistrationPeriodRequest<RegistrationPeriodDto>, PaginatedResult<List<RegistrationPeriodDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListRegistrationPeriodRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<RegistrationPeriodDto>>> Handle(ListRegistrationPeriodRequest<RegistrationPeriodDto> request, CancellationToken cancellationToken)
        {
            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<RegistrationPeriod>().GetAllInclude();

            if (request.IsAllDetail)
            {
                query = _unitOfWork.Repository<RegistrationPeriod>().AddInclude(query, x => x.Faculty);
            }
            else
            {
                if (request.IsGetFaculty == true)
                {
                    query = _unitOfWork.Repository<RegistrationPeriod>().AddInclude(query, x => x.Faculty);
                }
            }

            query = _sieveProcessor.Apply(sieve, query);

            var periods = await query.ToListAsync();

            var mapPeriods = _mapper.Map<List<RegistrationPeriodDto>>(periods);
            return PaginatedResult<List<RegistrationPeriodDto>>.Create(
                mapPeriods, 0, request.Page,
                request.PageSize, (int)HttpStatusCode.OK);
        }
    }
}
