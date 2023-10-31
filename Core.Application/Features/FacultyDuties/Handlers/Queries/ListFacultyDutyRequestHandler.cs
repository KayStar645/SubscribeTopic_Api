using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.FacultyDuty;
using Core.Application.Features.FacultyDuties.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Features.FacultyDuties.Handlers.Queries
{
    public class ListFacultyDutyRequestHandler : IRequestHandler<ListFacultyDutyRequest, PaginatedResult<List<FacultyDutyDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListFacultyDutyRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<FacultyDutyDto>>> Handle(ListFacultyDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListFacultyDutyDtoValidator(_unitOfWork, request.departmentId);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<FacultyDutyDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<FacultyDuty>().GetAllInclude();

            if (request.departmentId != null)
            {
                query = query.Where(x => x.DepartmentId == request.departmentId);
            }
            else if (request.facultyId != null)
            {
                query = query.Where(x => x.FacultyId == request.facultyId);
            }

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<FacultyDuty>().AddInclude(query, x => x.Faculty);
                query = _unitOfWork.Repository<FacultyDuty>().AddInclude(query, x => x.Department);
            }
            else
            {
                if (request.isGetFaculty == true)
                {
                    query = _unitOfWork.Repository<FacultyDuty>().AddInclude(query, x => x.Faculty);
                }
                if (request.isGetDepartment == true)
                {
                    query = _unitOfWork.Repository<FacultyDuty>().AddInclude(query, x => x.Department);
                }
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var periods = await query.ToListAsync();

            var mapPeriods = _mapper.Map<List<FacultyDutyDto>>(periods);
            return PaginatedResult<List<FacultyDutyDto>>.Success(
                mapPeriods, totalCount, request.page,
                request.pageSize);
        }
    }
}
