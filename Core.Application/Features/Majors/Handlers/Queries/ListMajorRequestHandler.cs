using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Industry;
using Core.Application.DTOs.Major;
using Core.Application.Features.Majors.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Majors.Handlers.Queries
{
    public class ListMajorRequestHandler : IRequestHandler<ListMajorRequest, PaginatedResult<List<MajorDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public ListMajorRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<MajorDto>>> Handle(ListMajorRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListMajorDtoValidator(_unitOfWork, request.industryId);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<MajorDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Major>().GetAllInclude();

            if(request.industryId != null)
            {
                query = query.Where(x => x.IndustryId == request.industryId);
            }
            else if (request.facultyId != null)
            {
                query = query.Where(x => x.Industry.FacultyId == request.facultyId);
            }

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<Major>().AddInclude(query, x => x.Industry);
            }
            else
            {
                if (request.isGetIndustry == true)
                {
                    query = _unitOfWork.Repository<Major>().AddInclude(query, x => x.Industry);
                }
            }

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var majors = await query.ToListAsync();

            var mapMajors = _mapper.Map<List<MajorDto>>(majors);

            return PaginatedResult<List<MajorDto>>.Success(
                mapMajors, totalCount, request.page,
                request.pageSize);
        }
    }
}
