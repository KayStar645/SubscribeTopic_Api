﻿using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.Features.RegistrationPeriods.Requests.Queries;
using Core.Application.Interfaces.Repositories;
using Core.Application.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.RegistrationPeriods.Handlers.Queries
{
    public class ListRegistrationPeriodRequestHandler : IRequestHandler<ListRegistrationPeriodRequest, PaginatedResult<List<RegistrationPeriodDto>>>
    {
        private readonly IRegistrationPeriodRepository _registrationPeriodRepo;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IUnitOfWork _unitOfWork;

        public ListRegistrationPeriodRequestHandler(IRegistrationPeriodRepository registrationPeriodRepository, IMapper mapper, ISieveProcessor sieveProcessor, IUnitOfWork unitOfWork)
        {
            _registrationPeriodRepo = registrationPeriodRepository;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedResult<List<RegistrationPeriodDto>>> Handle(ListRegistrationPeriodRequest request, CancellationToken cancellationToken)
        {
            var validator = new RegistrationPeriodDtoValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<RegistrationPeriodDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _registrationPeriodRepo.GetAllInclude();

            if (request.isAllDetail)
            {
                query = _registrationPeriodRepo.AddInclude(query, x => x.Faculty);
            }
            else
            {
                if (request.IsGetFaculty == true)
                {
                    query = _registrationPeriodRepo.AddInclude(query, x => x.Faculty);
                }
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var periods = await query.ToListAsync();

            var mapPeriods = _mapper.Map<List<RegistrationPeriodDto>>(periods);
            return PaginatedResult<List<RegistrationPeriodDto>>.Success(
                mapPeriods, totalCount, request.page,
                request.pageSize);
        }
    }
}
