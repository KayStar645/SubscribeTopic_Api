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
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Queries
{
    public class ListThesisRegistrationQueryHandler : IRequestHandler<ListThesisRegistrationRequest, PaginatedResult<List<ThesisRegisteredDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public IHttpContextAccessor _httpContextAccessor;

        public ListThesisRegistrationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContextAccessor = httpContextAccessor;
        }

        // Hiển thị 2 dạng
        // + Tất cả đề tài trong đợt đăng ký
        // + Những đề tài mà nhóm có thể đăng ký
        public async Task<PaginatedResult<List<ThesisRegisteredDto>>> Handle(ListThesisRegistrationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var type = _httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;
                var userName = _httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.UserName)?.Value;
                var facultyId = _httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.FacultyId)?.Value;

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
                var query = _unitOfWork.Repository<Thesis>().GetAllInclude();                
                query = query.Where(x => x.Status == Thesis.STATUS_APPROVED);

                // Lấy tất cả đề tài trong đợt hiện tại đã được duyệt (phải có trong nhiệm vụ của khoa)

                /* Nên có Trạng thái đề tài đăng ký (đk được hay không) + Message thông báo lỗi
                    + Đề tài có thể đăng ký được
                    + Đề tài không thể đăng ký do số lượng thành viên không hợp lệ
                    + Đề tài không thể đăng ký do nhóm khác đăng ký rồi
                    + Đề tài không đăng ký được do không phù hợp chuyên ngành
                    + 

                 */

                //if(request.isAllDetail != true)
                //{
                //    // Kiểm tra số lượng thành viên nhóm
                //    var countMember = await _unitOfWork.Repository<StudentJoin>()
                //                                       .Query()
                //                                       .Where(x => x.RegistrationPeriodId == period.Id)
                //                                       .Include(x => x.Student)
                //                                       .Where(x => x.Student.InternalCode == userName)
                //                                       .Include(x => x.Group)
                //                                       .Select(x => x.Group.CountMember)
                //                                       .FirstOrDefaultAsync();

                //    query = query.Where(x => x.MinQuantity <= countMember && countMember <= x.MaxQuantity);
                //}

                query = _sieveProcessor.Apply(sieve, query);

                int totalCount = await query.CountAsync();

                var thesiss = await query.ToListAsync();

                var mapThesiss = _mapper.Map<List<ThesisRegisteredDto>>(thesiss);

                return PaginatedResult<List<ThesisRegisteredDto>>.Success(
                    mapThesiss, totalCount, request.page,
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
