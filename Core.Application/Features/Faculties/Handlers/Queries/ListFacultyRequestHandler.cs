using AutoMapper;
using Core.Application.Contracts.Persistence;
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
    public class ListFacultyRequestHandler : IRequestHandler<ListFacultyRequest<FacultyDto>, PaginatedResult<List<FacultyDto>>>
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
        public async Task<PaginatedResult<List<FacultyDto>>> Handle(ListFacultyRequest<FacultyDto> request, CancellationToken cancellationToken)
        {
            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Faculty>().GetAllInclude();

            if (request.IsAllDetail)
            {
                
            }
            else
            {
                
            }

            query = _sieveProcessor.Apply(sieve, query);

            var facultys = await query.ToListAsync();

            var mapFacultys = _mapper.Map<List<FacultyDto>>(facultys);
            return PaginatedResult<List<FacultyDto>>.Create(
                mapFacultys, 0, request.Page,
                request.PageSize, (int)HttpStatusCode.OK);
        }
    }
}
