using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Department;
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

            var validator = new ListBaseRequestValidator<FacultyDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<FacultyDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Faculty>().GetAllInclude();

            if (request.IsAllDetail)
            {
                query = _unitOfWork.Repository<Faculty>().AddInclude(query, x => x.Dean_Teacher);
                query = _unitOfWork.Repository<Faculty>().AddInclude(query, x => x.Departments);
            }
            else
            {
                if(request.isGetDean == true)
                {
                    query = _unitOfWork.Repository<Faculty>().AddInclude(query, x => x.Dean_Teacher);
                }    
                if(request.isGetDepartment == true)
                {
                    query = _unitOfWork.Repository<Faculty>().AddInclude(query, x => x.Departments);
                }
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
