using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.Thesiss.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;

namespace Core.Application.Features.Thesiss.Handlers.Queries
{
    public class ListThesisPossibleCouncilQueryHandler : IRequestHandler<ListThesisPossibleCouncilRequest, PaginatedResult<List<ThesisDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IHttpContextAccessor _httpContext;

        public ListThesisPossibleCouncilQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContext = httpContext;
        }

        public async Task<PaginatedResult<List<ThesisDto>>> Handle(ListThesisPossibleCouncilRequest request, CancellationToken cancellationToken)
        {
            var userType = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;
            var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_TEACHER)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }

            // Từ id của người dùng lấy ra id của khoa
            var facultyId = await _unitOfWork.Repository<Teacher>()
                                    .Query()
                                    .Where(x => x.UserId == int.Parse(userId))
                                    .Include(x => x.Department)
                                    .Select(x => x.Department.FacultyId)
                                    .FirstAsync();

            // Lấy đợt đăng ký hiện tại của khoa
            var periodCurrentId = await _unitOfWork.Repository<RegistrationPeriod>()
                                        .Query()
                                        .Where(x => x.FacultyId == facultyId && x.IsActive == true)
                                        .Select(x => x.Id)
                                        .FirstOrDefaultAsync();

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Thesis>().GetAllInclude()
                .Where(x => x.Status == Thesis.STATUS_APPROVED && 
                            x.Duty.ForDuty.RegistrationPeriod.Id == periodCurrentId &&
                            (x.ThesisReviews.All(x => x.IsApprove == true) && x.ThesisReviews.Count > 0) &&
                            (x.ThesisInstructions.All(x => x.IsApprove == true) && x.ThesisInstructions.Count > 0) &&
                            x.CouncilId == null);
            
            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);


            var thesiss = await query.ToListAsync();

            var mapThesiss = _mapper.Map<List<ThesisDto>>(thesiss);

            return PaginatedResult<List<ThesisDto>>.Success(
                mapThesiss, totalCount, request.page,
                request.pageSize);
        }
    }
}
