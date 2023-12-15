using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Group;
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
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;
using System.Net.Http;

namespace Core.Application.Features.Thesiss.Handlers.Queries
{
    public class ListThesisRegistrationQueryHandler : IRequestHandler<ListThesisRegistrationRequest, PaginatedResult<List<ThesisRegisteredDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public IHttpContextAccessor _httpContext;

        public ListThesisRegistrationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContext = httpContextAccessor;
        }

        // Hiển thị 2 dạng
        // + Tất cả đề tài trong đợt đăng ký
        // + Những đề tài mà nhóm có thể đăng ký
        public async Task<PaginatedResult<List<ThesisRegisteredDto>>> Handle(ListThesisRegistrationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var type = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;
                var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                var userName = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.UserName)?.Value;
                var facultyId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.FacultyId)?.Value;

                if(type != CLAIMS_VALUES.TYPE_STUDENT)
                {
                    throw new UnauthorizedException((int)HttpStatusCode.Forbidden);
                }
                // Lấy đợt đăng ký hiện tại
                var period = await _unitOfWork.Repository<RegistrationPeriod>()
                                              .Query()
                                              .Where(x => x.FacultyId == int.Parse(facultyId) && x.IsActive &&
                                                     x.TimeStart <= DateTime.Now && DateTime.Now <= x.TimeEnd)
                                              .FirstOrDefaultAsync();
                if (period == null)
                {
                    throw new UnauthorizedException((int)HttpStatusCode.Forbidden);
                }

                var sieve = _mapper.Map<SieveModel>(request);
                // Tất cả đề tài của đợt hiện tại đã được duyệt của khoa mình
                var query = _unitOfWork.Repository<Thesis>().Query().Where(x => x.Status == Thesis.STATUS_APPROVED);
                query = query.Where(x => x.Duty.Department.FacultyId == int.Parse(facultyId));
                query = query.Where(x => x.Duty.ForDuty.Faculty.RegistrationPeriods.Any(x => x.Id == period.Id));

                query = _sieveProcessor.Apply(sieve, query);

                int totalCount = await query.CountAsync();

                var listThesis = await query.ToListAsync();

                var mapThesis = _mapper.Map<List<ThesisRegisteredDto>>(listThesis);


                /* Nên có Trạng thái đề tài đăng ký (đk được hay không) + Message thông báo lỗi
                    + Đề tài có thể đăng ký được
                    + Đề tài không thể đăng ký do số lượng thành viên không hợp lệ
                    + Đề tài không thể đăng ký do nhóm khác đăng ký rồi
                    + Đề tài không đăng ký được do không phù hợp chuyên ngành
                    + 
                 */
                // Group me của đợt hiện tại
                var subQuery = _unitOfWork.Repository<Group>().Query()
                                     .Join(
                                         _unitOfWork.Repository<StudentJoin>().Query(),
                                         g => g.Id,
                                         sj => sj.GroupId,
                                         (g, sj) => new { Group = g, StudentJoin = sj }
                                     )
                                     .Where(joined => joined.StudentJoin.Student.UserId == int.Parse(userId) &&
                                                      joined.StudentJoin.RegistrationPeriodId == period.Id)
                                     .Select(joined => joined.Group);
                var group = await subQuery.FirstOrDefaultAsync();
                if(group == null )
                {
                    throw new UnauthorizedException((int)HttpStatusCode.Forbidden);
                }    

                // Lấy trạng thái và lý do không đăng ký được đề tài
                foreach (var thesis in mapThesis)
                {
                    thesis.IsRegister = true;
                    thesis.Messages = new List<string>();
                    // Nhóm khác đk chưa
                    var thesisRegis = await _unitOfWork.Repository<ThesisRegistration>().Query()
                                                .Where(x => x.ThesisId == thesis.Id)
                                                .Include(x => x.Group)
                                                .FirstOrDefaultAsync();
                    if(thesisRegis != null)
                    {
                        thesis.IsRegister = false;
                        thesis.Messages.Add("Đề tài đã được nhóm khác đăng ký!");
                        thesis.GroupDto = _mapper.Map<GroupDto>(thesisRegis.Group);
                    }    

                    // Số lượng thành viên
                    if (group.CountMember < thesis.MinQuantity || thesis.MaxQuantity < group.CountMember)
                    {
                        thesis.IsRegister = false;
                        thesis.Messages.Add("Số lượng thành viên của nhóm không đúng với yêu cầu của đề tài!");
                    }

                    // Dữ liệu kèm
                    var thesisInstructions = await _unitOfWork.Repository<ThesisInstruction>()
                                                .Query()
                                                .Where(x => x.ThesisId == thesis.Id)
                                                .Include(x => x.Teacher)
                                                .Select(x => x.Teacher)
                                                .ToListAsync();
                    thesis.ThesisInstructions = _mapper.Map<List<TeacherDto>>(thesisInstructions);

                    var thesisMajors = await _unitOfWork.Repository<ThesisMajor>()
                                                                        .Query()
                                                                        .Where(x => x.ThesisId == thesis.Id)
                                                                        .Include(x => x.Major)
                                                                        .Select(x => x.Major)
                                                                        .ToListAsync();
                    thesis.ThesisMajors = _mapper.Map<List<MajorDto>>(thesisMajors);

                }    

                return PaginatedResult<List<ThesisRegisteredDto>>.Success(
                    mapThesis, totalCount, request.page,
                    request.pageSize);
            }

            catch (NotFoundException ex)
            {
                return PaginatedResult<List<ThesisRegisteredDto>>.Failure((int)HttpStatusCode.NotFound, new List<string> { ex.Message });
            }
            catch (BadRequestException ex)
            {
                return PaginatedResult<List<ThesisRegisteredDto>>.Failure((int)HttpStatusCode.BadRequest, new List<string> { ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return PaginatedResult<List<ThesisRegisteredDto>>.Failure(ex.ErrorCode, new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<ThesisRegisteredDto>>.Failure((int)HttpStatusCode.InternalServerError, new List<string> { ex.Message });
            }
        }
    }
}
