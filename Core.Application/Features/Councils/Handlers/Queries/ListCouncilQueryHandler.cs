using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Commissioner;
using Core.Application.DTOs.Council;
using Core.Application.Features.Councils.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Councils.Handlers.Queries
{
    public class ListCouncilQueryHandler : IRequestHandler<ListCouncilRequest, PaginatedResult<List<CouncilDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListCouncilQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<CouncilDto>>> Handle(ListCouncilRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListCouncilValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<CouncilDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Council>().GetAllInclude()
                                   .Where(x => x.FacultyId == request.facultyId);

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var councils = await query.ToListAsync();

            var mapCouncils = _mapper.Map<List<CouncilDto>>(councils);
            foreach (var council in mapCouncils)
            {
                var com = await _unitOfWork.Repository<Commissioner>()
                            .Query().Where(x => x.CouncilId == council.Id)
                            .Include(x => x.Teacher)
                            .ToListAsync();
                council.Commissioners = _mapper.Map<List<CommissionerDto>>(com);
            }

            return PaginatedResult<List<CouncilDto>>.Success(
                mapCouncils, totalCount, request.page,
                request.pageSize);
        }
    }
}
