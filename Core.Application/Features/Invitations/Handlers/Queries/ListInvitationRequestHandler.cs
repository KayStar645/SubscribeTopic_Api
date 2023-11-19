using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Invitation;
using Core.Application.Features.Invitations.Request.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
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
        public ListInvitationRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
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

            query = query.Where(x => x.GroupId == request.groupId);

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

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var invitations = await query.ToListAsync();

            var mapInvitations = _mapper.Map<List<InvitationDto>>(invitations);

            return PaginatedResult<List<InvitationDto>>.Success(
                mapInvitations, totalCount, request.page,
                request.pageSize);
        }
    }
}
