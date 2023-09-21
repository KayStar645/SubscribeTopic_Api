using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
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
    public class ListStudentJoinRequestHandler : IRequestHandler<ListMajorRequest<MajorDto>, PaginatedResult<List<MajorDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public ListStudentJoinRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<MajorDto>>> Handle(ListMajorRequest<MajorDto> request, CancellationToken cancellationToken)
        {
            var validator = new ListBaseRequestValidator<MajorDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<MajorDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Major>().GetAllInclude();

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<Major>().AddInclude(query, x => x.Faculty);
            }
            else
            {
                if (request.isGetFaculty == true)
                {
                    query = _unitOfWork.Repository<Major>().AddInclude(query, x => x.Faculty);
                }
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var majors = await query.ToListAsync();

            var mapMajors = _mapper.Map<List<MajorDto>>(majors);

            return PaginatedResult<List<MajorDto>>.Success(
                mapMajors, totalCount, request.page,
                request.pageSize);
        }
    }
}
