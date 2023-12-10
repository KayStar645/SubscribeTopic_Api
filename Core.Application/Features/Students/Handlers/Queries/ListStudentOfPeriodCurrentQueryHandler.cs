using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Student;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.Students.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Students.Handlers.Queries
{
    public class ListStudentOfPeriodCurrentQueryHandler : IRequestHandler<ListStudentOfPeriodCurrentRequest, PaginatedResult<List<FriendDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IHttpContextAccessor _httpContext;
        public ListStudentOfPeriodCurrentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContext = httpContext;
        }

        // Phải có trạng thái lời mời kết bạn
        public async Task<PaginatedResult<List<FriendDto>>> Handle(ListStudentOfPeriodCurrentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validator = new ListStudentOfPeriodCurrentValidator(_unitOfWork, request.majorId, request.industryId);
                var result = await validator.ValidateAsync(request);

                if (result.IsValid == false)
                {
                    var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return PaginatedResult<List<FriendDto>>
                        .Failure((int)HttpStatusCode.BadRequest, errorMessages);
                }

                var sieve = _mapper.Map<SieveModel>(request);

                var query = _unitOfWork.Repository<Student>().GetAllInclude();

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
                query = query.Where(x => x.Major.Industry.FacultyId == facultyId);

                // Đợt đăng ký hiện tại đang được cấu hình
                var periodId = await _unitOfWork.Repository<RegistrationPeriod>()
                                                .Query()
                                                .Where(x => x.FacultyId == facultyId && x.IsActive == true)
                                                .Select(x => x.Id)
                                                .FirstOrDefaultAsync();
                if (periodId == null)
                {
                    return PaginatedResult<List<FriendDto>>.Success(null, 0, request.page, request.pageSize);
                }
                query = query.Where(x => x.Major.Industry.Faculty.RegistrationPeriods.Any(p => p.Id == periodId));

                // Không phải tôi
                query = query.Where(x => x.UserId != int.Parse(userId));

                // Các thông tin lọc bổ sung
                if (request.majorId != null)
                {
                    query = query.Where(x => x.MajorId == request.majorId);
                }
                else if (request.industryId != null)
                {
                    query = query.Where(x => x.Major.IndustryId == request.industryId);
                }

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<Student>().AddInclude(query, x => x.Major);
                }
                else
                {
                    if (request.isGetMajor == true)
                    {
                        query = _unitOfWork.Repository<Student>().AddInclude(query, x => x.Major);
                    }
                }

                int totalCount = await query.CountAsync();

                query = _sieveProcessor.Apply(sieve, query);

                var students = await query.ToListAsync();

                var studentJoin = await _unitOfWork.Repository<StudentJoin>().Query()
                                        .Where(m => m.Student.Major.Industry.Faculty.RegistrationPeriods
                                                        .Any(r => r.Id == periodId && r.IsActive == true))
                                        .FirstOrDefaultAsync();
                if (studentJoin == null)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }

                var myGroup = await _unitOfWork.Repository<Group>().Query()
                                    .Where(g => g.Id == studentJoin.GroupId)
                                    .FirstOrDefaultAsync();

                if (myGroup == null)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }
                var members = await _unitOfWork.Repository<StudentJoin>().Query()
                                        .Where(s => s.GroupId == myGroup.Id)
                                        .ToListAsync();
                myGroup.Members = members;

                myGroup.Invitations = await _unitOfWork.Repository<Invitation>().Query()
                                                        .Where(x => x.GroupId == myGroup.Id)
                                                        .Include(x => x.StudentJoin)
                                                        .ToListAsync();

                var mapStudents = students.Select(s =>
                {
                    var friendDto = _mapper.Map<FriendDto>(s);

                    friendDto.Status = DetermineStatus(s, myGroup);
                    var studentJoin = myGroup.Members.FirstOrDefault(m => m.StudentId == s.Id);

                    if (studentJoin != null)
                    {
                        friendDto.StudentJoinId = studentJoin.Id;
                    }

                    return friendDto;
                }).ToList();


                return PaginatedResult<List<FriendDto>>.Success(mapStudents, totalCount, request.page, request.pageSize);
            }
            catch (NotFoundException ex)
            {
                return PaginatedResult<List<FriendDto>>.Failure((int)HttpStatusCode.NotFound, new List<string>() { ex.Message });
            }
            catch (BadRequestException ex)
            {
                return PaginatedResult<List<FriendDto>>.Failure((int)HttpStatusCode.BadRequest, new List<string>() { ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return PaginatedResult<List<FriendDto>>.Failure(ex.ErrorCode, new List<string>() { ex.Message });
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<FriendDto>>.Failure((int)HttpStatusCode.InternalServerError, new List<string>() { ex.Message });
            }
        }

        private string DetermineStatus(Student student, Group myGroup)
        {
            if (myGroup.Members.Any(m => m.StudentId == student.Id))
            {
                return FriendDto.STATUS_APPROVE;
            }
            else if (myGroup.Invitations.Any(i => i.StudentJoin.StudentId == student.Id))
            {
                return FriendDto.STATUS_SEND;
            }
            else
            {
                return FriendDto.STATUS_NEW;
            }
        }
    }
}
