using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Group;
using Core.Application.DTOs.Student;
using Core.Application.DTOs.StudentJoin;
using Core.Application.Exceptions;
using Core.Application.Features.Groups.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Groups.Handlers.Queries
{
    public class DetailGroupQueryHandler : IRequestHandler<DetailGroupRequest, Result<GroupDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContext;

        public DetailGroupQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
        }

        public async Task<Result<GroupDto>> Handle(DetailGroupRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailGroupDtoValidator(request.isGetGroupMeCurrent);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<GroupDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Group>().GetByIdInclude(request.id);
                if (request.isGetGroupMeCurrent == true)
                {
                    var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                    var userType = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

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
                        return Result<GroupDto>.Failure(
                                ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                                (int)HttpStatusCode.NotFound
                            );
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

                var findGroup = await query.SingleAsync();

                if (findGroup is null)
                {
                    return Result<GroupDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var groupDto = _mapper.Map<GroupDto>(findGroup);

                if (request.isGetMember == true || request.isAllDetail == true)
                {
                    var members = await _unitOfWork.Repository<StudentJoin>()
                                        .Query()
                                        .Where(x => x.GroupId == findGroup.Id)
                                        .Include(x => x.Student)
                                        .ToListAsync();

                    groupDto.Members = _mapper.Map<List<StudentJoinDto>>(members);
                }

                return Result<GroupDto>.Success(groupDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<GroupDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
