using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Duty;
using Core.Application.Features.Duties.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Duties.Handlers.Queries
{
    public class ListDutyQueryHandler : IRequestHandler<ListDutyRequest, PaginatedResult<List<DutyDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public ListDutyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<DutyDto>>> Handle(ListDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListDutyValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<DutyDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Duty>().GetAllInclude();

            query = query.Where(x => x.Type == request.type);

            if (request.isAllDetail || request.isGetFaculty == true)
            {
                query = query.Include(x => x.Faculty);
            }
            if (request.isAllDetail || request.isGetDepartment == true)
            {
                query = query.Include(x => x.Department);
            }
            if (request.isAllDetail || request.isGetTeacher == true)
            {
                query = query.Include(x => x.Teacher);
            }

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var duties = await query.ToListAsync();

            var mapDuties = _mapper.Map<List<DutyDto>>(duties);

            return PaginatedResult<List<DutyDto>>.Success(
                mapDuties, totalCount, request.page,
                request.pageSize);
        }
    }
}
