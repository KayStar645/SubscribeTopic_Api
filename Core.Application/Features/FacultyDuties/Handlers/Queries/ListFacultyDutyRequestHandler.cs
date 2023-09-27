using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Notification;
using Core.Application.DTOs.FacultyDuty;
using Core.Application.Features.FacultyDuties.Requests.Queries;
using Core.Application.Interfaces.Repositories;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;
using Core.Application.DTOs.Department;
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
            var validator = new ListBaseRequestValidator<FacultyDutyDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<FacultyDutyDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var validationContext = new ValidationContext(request, null, null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (isValid == false)
            {
                var errorMessages = validationResults.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<FacultyDutyDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<FacultyDuty>().GetAllInclude()
                                   .Where(x => x.FacultyId == request.facultyId);

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<FacultyDuty>().AddInclude(query, x => x.Faculty);
            }
            else
            {
                if (request.isGetFaculty == true)
                {
                    query = _unitOfWork.Repository<FacultyDuty>().AddInclude(query, x => x.Faculty);
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
