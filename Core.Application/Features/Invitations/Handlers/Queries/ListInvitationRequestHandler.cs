using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Invitation;
using Core.Application.Exceptions;
using Core.Application.Features.Invitations.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Invitations.Handlers.Queries
{
    public class ListInvitationRequestHandler : IRequestHandler<ListInvitationRequest, PaginatedResult<List<InvitationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public IHttpContextAccessor _httpContext;
        public ListInvitationRequestHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContext = httpContext;
        }

        public async Task<PaginatedResult<List<InvitationDto>>> Handle(ListInvitationRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListInvitationDtoValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<InvitationDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Invitation>().GetAllInclude();

            var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var userType = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_STUDENT)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }
            // Khoa của tôi
            var facultyId = await _unitOfWork.Repository<Student>()
                                           .Query()
                                           .Where(x => x.UserId == int.Parse(userId))
                                           .Include(x => x.Major)
                                           .ThenInclude(x => x.Industry)
                                           .Select(x => x.Major.Industry.FacultyId)
                                           .FirstOrDefaultAsync();

            var periodId = await _unitOfWork.Repository<RegistrationPeriod>()
                                    .Query()
                                    .Where(x => x.FacultyId == facultyId && x.IsActive == true)
                                    .Select(x => x.Id)
                                    .FirstOrDefaultAsync();
            if (periodId == null)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }

            // Lấy danh sách lời mời theo đợt hiện tại
            query = query.Where(x => x.StudentJoin.RegistrationPeriodId == periodId);

            // Lấy danh sách lời mời sinh viên này được nhận
            query = query.Where(x => x.Status == Invitation.STATUS_SENT && 
                            x.StudentJoin.Student.UserId == int.Parse(userId));

            if (request.isAllDetail)
            {
                query = query.Include(x => x.Group);
                query = query.Include(x => x.StudentJoin)
                                .ThenInclude(x => x.Student);
            }
            else
            {
                if (request.isGetGroup == true)
                {
                    query = query.Include(x => x.Group);
                }
                if (request.isGetStudentJoin == true)
                {
                    query = query.Include(x => x.StudentJoin)
                                    .ThenInclude(x => x.Student);
                }
            }

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var invitations = await query.ToListAsync();

            var mapInvitations = _mapper.Map<List<InvitationDto>>(invitations);

            return PaginatedResult<List<InvitationDto>>.Success(
                mapInvitations, totalCount, request.page,
                request.pageSize);
        }
    }
}
