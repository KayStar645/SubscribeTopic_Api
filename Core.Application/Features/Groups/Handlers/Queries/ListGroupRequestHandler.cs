using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Group;
using Core.Application.Features.Groups.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Groups.Handlers.Queries
{
    public class ListGroupRequestHandler : IRequestHandler<ListGroupRequest, PaginatedResult<List<GroupDto>>>
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListGroupRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<GroupDto>>> Handle(ListGroupRequest request, CancellationToken cancellationToken)
        {

            var validator = new ListBaseRequestValidator<GroupDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<GroupDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Group>().GetAllInclude();
            
            query = query.Where(x => x.Leader.Student.Major.Industry.FacultyId == request.facultyId);

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<Group>().AddInclude(query, x => x.Leader.Student);
            }
            else
            {
                if (request.isGetLeader == true)
                {
                    query = _unitOfWork.Repository<Group>().AddInclude(query, x => x.Leader.Student);
                }
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var groups = await query.ToListAsync();

            var mapGroups = _mapper.Map<List<GroupDto>>(groups);
            return PaginatedResult<List<GroupDto>>.Success(
                mapGroups, totalCount, request.page,
                request.pageSize);
        }
    }
}
