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
            var validator = new ListBaseRequestValidator<DepartmentDutyDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<DepartmentDutyDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var validationContext = new ValidationContext(request, null, null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (isValid == false)
            {
                var errorMessages = validationResults.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<DepartmentDutyDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<DepartmentDuty>().GetAllInclude()
                                   .Where(x => x.DepartmentId == request.departmentId);

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<DepartmentDuty>().AddInclude(query, x => x.Department);
            }
            else
            {
                if (request.isGetDepartment == true)
                {
                    query = _unitOfWork.Repository<DepartmentDuty>().AddInclude(query, x => x.Department);
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
