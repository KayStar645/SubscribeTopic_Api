﻿using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Group;
using Core.Application.Exceptions;
using Core.Application.Features.Groups.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Groups.Handlers.Queries
{
    public class ListGroupQueryHandler : IRequestHandler<ListGroupRequest, PaginatedResult<List<GroupDto>>>
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IHttpContextAccessor _httpContext;

        public ListGroupQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContext = httpContext;
        }
        public async Task<PaginatedResult<List<GroupDto>>> Handle(ListGroupRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListGroupDtoValidator(_unitOfWork, request.isGetGroupMe);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<GroupDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Group>().GetAllInclude();

            if (request.isGetGroupMe == true)
            {
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
                query = _unitOfWork.Repository<Group>().Query()
                                 .Join(
                                     _unitOfWork.Repository<StudentJoin>().Query(),
                                     g => g.Id,
                                     sj => sj.GroupId,
                                     (g, sj) => new { Group = g, StudentJoin = sj }
                                 )
                                 .Where(joined => joined.StudentJoin.Student.UserId == int.Parse(userId) &&
                                                  joined.StudentJoin.RegistrationPeriodId == periodId)
                                 .Select(joined => joined.Group);
            }
            else
            {
                query = query.Where(x => x.Leader.Student.Major.Industry.FacultyId == request.facultyId);
            }

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

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var groups = await query.ToListAsync();

            var mapGroups = _mapper.Map<List<GroupDto>>(groups);
            return PaginatedResult<List<GroupDto>>.Success(
                mapGroups, totalCount, request.page,
                request.pageSize);
        }
    }
}
