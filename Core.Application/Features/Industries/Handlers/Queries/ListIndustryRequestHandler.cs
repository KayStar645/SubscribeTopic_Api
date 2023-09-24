using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Industry;
using Core.Application.Features.Industries.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Industries.Handlers.Queries
{
    internal class ListIndustryRequestHandler : IRequestHandler<ListIndustryRequest, PaginatedResult<List<IndustryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public ListIndustryRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<IndustryDto>>> Handle(ListIndustryRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListBaseRequestValidator<IndustryDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<IndustryDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Industry>().GetAllInclude();

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<Industry>().AddInclude(query, x => x.Faculty);
            }
            else
            {
                if (request.isGetFaculty == true)
                {
                    query = _unitOfWork.Repository<Industry>().AddInclude(query, x => x.Faculty);
                }
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var Industrys = await query.ToListAsync();

            var mapIndustrys = _mapper.Map<List<IndustryDto>>(Industrys);

            return PaginatedResult<List<IndustryDto>>.Success(
                mapIndustrys, totalCount, request.page,
                request.pageSize);
        }
    }
}
