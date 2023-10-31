using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.DepartmentDuty;
using Core.Application.Features.DepartmentDuties.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Features.DepartmentDuties.Handlers.Queries
{
    public class ListDepartmentDutyRequestHandler : IRequestHandler<ListDepartmentDutyRequest, PaginatedResult<List<DepartmentDutyDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListDepartmentDutyRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<DepartmentDutyDto>>> Handle(ListDepartmentDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListDepartmentDutyDtoValidator(_unitOfWork, request.teacherId);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<DepartmentDutyDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<DepartmentDuty>().GetAllInclude();

            if (request.teacherId != null)
            {
                query = query.Where(x => x.TeacherId == request.teacherId);
            }
            else if (request.departmentId != null)
            {
                query = query.Where(x => x.DepartmentId == request.departmentId);
            }

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<DepartmentDuty>().AddInclude(query, x => x.Department);
                query = _unitOfWork.Repository<DepartmentDuty>().AddInclude(query, x => x.Teacher);
            }
            else
            {
                if (request.isGetDepartment == true)
                {
                    query = _unitOfWork.Repository<DepartmentDuty>().AddInclude(query, x => x.Department);
                }
                if (request.isGetTeacher == true)
                {
                    query = _unitOfWork.Repository<DepartmentDuty>().AddInclude(query, x => x.Teacher);
                }
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var periods = await query.ToListAsync();

            var mapPeriods = _mapper.Map<List<DepartmentDutyDto>>(periods);
            return PaginatedResult<List<DepartmentDutyDto>>.Success(
                mapPeriods, totalCount, request.page,
                request.pageSize);
        }
    }
}
