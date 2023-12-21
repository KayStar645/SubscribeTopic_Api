using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.Thesiss.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Queries
{
    public class DetailThesisMePeriodCurrentQueryHandler : IRequestHandler<DetailThesisMePeriodCurrentRequest, Result<ThesisDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContext;

        public DetailThesisMePeriodCurrentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IHttpContextAccessor httpContext)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
        }

        public async Task<Result<ThesisDto>> Handle(DetailThesisMePeriodCurrentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Đợt đăng ký hiện tại
                var userType = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;
                var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                if (userType != CLAIMS_VALUES.TYPE_STUDENT)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }

                // Từ id của người dùng lấy ra id của khoa
                var facultyId = await _unitOfWork.Repository<Student>()
                                    .Query()
                                    .Where(x => x.UserId == int.Parse(userId))
                                    .Include(x => x.Major.Industry)
                                    .Select(x => x.Major.Industry.FacultyId)
                                    .FirstOrDefaultAsync();

                // Lấy đợt đăng ký hiện tại của khoa
                var periodCurrentId = await _unitOfWork.Repository<RegistrationPeriod>()
                                            .Query()
                                            .Where(x => x.FacultyId == facultyId && x.IsActive == true)
                                            .Select(x => x.Id)
                                            .FirstOrDefaultAsync();

                // Nhóm của tôi ở đợt hiện tại
                var groupId = await _unitOfWork.Repository<StudentJoin>()
                                .Query()
                                .Where(x => x.Student.UserId == int.Parse(userId) && x.RegistrationPeriodId == periodCurrentId)
                                .Select(x => x.GroupId)
                                .FirstOrDefaultAsync();

                // Đề tài của nhóm tôi

                var query = _unitOfWork.Repository<Thesis>()
                                .Query()
                                .Where(x => x.ThesisRegistration.GroupId == groupId);
                
                var findThesis = await query.SingleOrDefaultAsync();

                var thesisDto = _mapper.Map<ThesisDto>(findThesis);

                var thesisInstructions = await _unitOfWork.Repository<ThesisInstruction>()
                                                .Query()
                                                .Where(x => x.ThesisId == findThesis.Id)
                                                .Include(x => x.Teacher)
                                                .Select(x => x.Teacher)
                                                .ToListAsync();

                thesisDto.ThesisInstructions = _mapper.Map<List<TeacherDto>>(thesisInstructions);

                var thesisMajors = await _unitOfWork.Repository<ThesisMajor>()
                                                .Query()
                                                .Where(x => x.ThesisId == findThesis.Id)
                                                .Include(x => x.Major)
                                                .Select(x => x.Major)
                                                .ToListAsync();

                thesisDto.ThesisMajors = _mapper.Map<List<MajorDto>>(thesisMajors);

                return Result<ThesisDto>.Success(thesisDto, (int)HttpStatusCode.OK);
            }
            catch (UnauthorizedException ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
