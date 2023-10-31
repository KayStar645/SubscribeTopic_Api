using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Faculty;
using Core.Application.Features.Faculties.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Faculties.Handlers.Queries
{
    public class ListFacultyRequestHandler : IRequestHandler<ListFacultyRequest, PaginatedResult<List<FacultyDto>>>
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListFacultyRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<FacultyDto>>> Handle(ListFacultyRequest request, CancellationToken cancellationToken)
        {

            var validator = new ListBaseRequestValidator<FacultyDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<FacultyDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Domain.Entities.Faculties>().GetAllInclude();

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<Domain.Entities.Faculties>().AddInclude(query, x => x.Dean_Teacher);
            }
            else
            {
                if(request.isGetDean == true)
                {
                    query = _unitOfWork.Repository<Domain.Entities.Faculties>().AddInclude(query, x => x.Dean_Teacher);
                }
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var faculties = await query.ToListAsync();

            var mapFaculties = _mapper.Map<List<FacultyDto>>(faculties);
            return PaginatedResult<List<FacultyDto>>.Success(
                mapFaculties, totalCount, request.page,
                request.pageSize);
        }
    }
}
